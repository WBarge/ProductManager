

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Data.EF.Helpers;
using ProductManager.Data.EF.Repos;
using ProductManager.Glue.Interfaces.Models;

namespace ProductManager.Data.EF.Tests.Repos
{
    [TestFixture,Description("Tests for the product repository")]
    public class ProductRepoTests
    {
        private IServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _serviceProvider = TestSetupHelper.GetServiceProvider();
        }

        [SetUp]
        public void Setup()
        {
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.SeedData();
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.RemoveData();
            }
        }

        [Test, Description("Test required context object")]
        public void Constructor_RequiredContext_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductRepo(null));
        }

        [Test, Description("Test required context object")]
        public void Constructor_RequiredContext_Success()
        {
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    ProductRepo sut = new ProductRepo(context);
                    sut.Should().NotBeNull();
                }
            }
        }

        [Test, Description("Test to see all records are returned")]
        public async Task FindPagedShortProductRecords_ReturnsTwoRecords_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    int expectedRecordCount = context.Products.Count();
                    ProductRepo sut = new ProductRepo(context);
                    
                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedShortProductRecordsAsync();

                    await TestContext.Out.WriteLineAsync("Examining results");
                    results.Should().NotBeEmpty("This test should return all products we seeded the db with");
                    results.Count().Should().Be(expectedRecordCount, "The query should return the same record count as the dbSet");
                }
            }
        }

        [Test, Description("Test to see that records are paged")]
        public async Task FindPagedShortProductRecords_ReturnsPagedRecords_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new ProductRepo(context);
                    
                    await TestContext.Out.WriteLineAsync("Executing first test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedShortProductRecordsAsync(null!,1,1);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    results.Should().NotBeEmpty("This test should return products we seeded the db with");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return 1 record");
                    IShortProduct? firstRecord = results.FirstOrDefault();
                    firstRecord.Should().NotBeNull();

                    await TestContext.Out.WriteLineAsync("Executing second test");
                    results = await sut.FindPagedShortProductRecordsAsync(null!,2,1);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    results.Should().NotBeEmpty("This test should return products we seeded the db with");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return 1 record");
                    IShortProduct? secondRecord = results.FirstOrDefault();
                    secondRecord.Should().NotBeNull();
                    firstRecord.Should().NotBeEquivalentTo(secondRecord,"we are testing paging and the records should not be the same");
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_ReturnsFilteredRecords_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");


            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new ProductRepo(context);

                    Dictionary<string, IFilterMetaData[]> filter = new Dictionary<string, IFilterMetaData[]>();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "T125",
                            MatchMode = FilteringEngine.EQUALS_COMPARISON,
                            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Sku",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedShortProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                }
            }
        }
    }
}