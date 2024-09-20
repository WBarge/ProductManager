

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Data.EF.Helpers;
using ProductManager.Data.EF.Model;
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
            Assert.Throws<ArgumentNullException>(() => new ProductRepo(null!));
        }

        [Test, Description("Test required context object")]
        public void Constructor_RequiredContext_Success()
        {
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    ProductRepo sut = new(context);
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
                    int expectedRecordCount = context.Products.Count(p => p.Deleted == false);
                    ProductRepo sut = new(context);
                    
                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IProduct> results = await sut.FindPagedProductRecordsAsync();

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
                    ProductRepo sut = new(context);
                    
                    await TestContext.Out.WriteLineAsync("Executing first test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(null!,1,1);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    results.Should().NotBeEmpty("This test should return products we seeded the db with");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return 1 record");
                    IShortProduct? firstRecord = results.FirstOrDefault();
                    firstRecord.Should().NotBeNull();

                    await TestContext.Out.WriteLineAsync("Executing second test");
                    results = await sut.FindPagedProductRecordsAsync(null!,2,1);

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
        public async Task FindPagedShortProductRecords_FilterEqualWithAndForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
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
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterEqualWithOrForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "T125",
                            MatchMode = FilteringEngine.EQUALS_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Sku",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

         [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterNotEqualWithAndForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 4;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "T125",
                            MatchMode = FilteringEngine.NOT_EQUALS_COMPARISON,
                            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Sku",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterNotEqualWithOrForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 4;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "T125",
                            MatchMode = FilteringEngine.NOT_EQUALS_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Sku",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterStartsWithAndForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "A Test three",
                            MatchMode = FilteringEngine.STARTS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterStartsWithOrForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "A Test three",
                            MatchMode = FilteringEngine.STARTS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

         [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterEndsWithAndForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "four Product",
                            MatchMode = FilteringEngine.ENDS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterEndsWithOrForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "four Product",
                            MatchMode = FilteringEngine.ENDS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterContainsWithAndForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "five",
                            MatchMode = FilteringEngine.CONTAINS_COMPARISON,
                            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterContainsWithOrForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "five",
                            MatchMode = FilteringEngine.CONTAINS_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterNotContainsWithAndForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 4;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "five",
                            MatchMode = FilteringEngine.NOT_CONTAINS_COMPARISON,
                            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterNotContainsWithOrForSingleField_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 4;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "five",
                            MatchMode = FilteringEngine.NOT_CONTAINS_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }

                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterWithOrForTwoFieldValues_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 2;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "A Test four",
                            MatchMode = FilteringEngine.STARTS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        },
                        new FilterMetaData()
                        {
                            SearchValue = "A Test five",
                            MatchMode = FilteringEngine.STARTS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }
                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    await TestContext.Out.WriteLineAsync(results.Count().ToString());
                    results.Should().NotBeEmpty("This test should return the products we are searching for");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterWithAndForTwoFieldValues_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 0;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "A Test four",
                            MatchMode = FilteringEngine.STARTS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
                        },
                        new FilterMetaData()
                        {
                            SearchValue = "A Test five",
                            MatchMode = FilteringEngine.STARTS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
                        }
                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    results.Should().NotBeNull("This test should return an empty product list when there are zero results and no error");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see filtered records are returned")]
        public async Task FindPagedShortProductRecords_FilterWithOrForTwoDifferentFieldValues_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    //two different fields naturally creates a logical and situation
                    const int EXPECTED_RECORD_COUNT = 1;
                    ProductRepo sut = new(context);

                    Dictionary<string, IFilterMetaData[]> filter = new();
                    List<FilterMetaData> filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "A Test five",
                            MatchMode = FilteringEngine.STARTS_WITH_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }
                    ];
                    filter.Add("Description",filterList.ToArray<IFilterMetaData>());

                    filterList =
                    [
                        new FilterMetaData()
                        {
                            SearchValue = "T5",
                            MatchMode = FilteringEngine.EQUALS_COMPARISON,
                            LogicalOperator = FilteringEngine.OR_LOGICAL_OPERATOR
                        }
                    ];
                    filter.Add("Sku",filterList.ToArray<IFilterMetaData>());

                    await TestContext.Out.WriteLineAsync("Executing test");
                    IEnumerable<IShortProduct> results = await sut.FindPagedProductRecordsAsync(filter);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    results.Should().NotBeNull("This test should return an empty product list when there are zero results and no error");
                    results.Count().Should().Be(EXPECTED_RECORD_COUNT, "The query should return the same record count as the dbSet");
                    
                }
            }
        }

        [Test, Description("Test to see records count returned")]
        public async Task GetProductCount_ReturnsCount_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope =
                   _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const int EXPECTED_RECORD_COUNT = 5;
                    ProductRepo sut = new(context);

                    await TestContext.Out.WriteLineAsync("Executing test");
                    long results = await sut.GetProductCountAsync();

                    await TestContext.Out.WriteLineAsync("Examining results");
                    results.Should().Be(EXPECTED_RECORD_COUNT);

                }
            }
        }

        [Test, Description("Test an instance is returned")]
        public async Task GetInstance_ReturnsAnInstanceOfIProduct_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope =
                   _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    ProductRepo sut = new(context);
                    await TestContext.Out.WriteLineAsync("Executing test");
                    IProduct result = sut.CreateInstance();
                    await TestContext.Out.WriteLineAsync("Examining results");
                    result.Should().NotBeNull();
                }
            }
        }

        [Test, Description("Test an instance is returned")]
        public async Task AddMinimumProductAsync_InsertsTheProduct_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope =
                   _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    const string  TEST_NAME = "TestValue";
                    ProductRepo sut = new(context);
                    IProduct data = sut.CreateInstance();
                    data.Should().NotBeNull();
                    data.Name = TEST_NAME;
                    data.Sku = TEST_NAME;
                    data.ShortDescription =TEST_NAME;
                    data.Description=TEST_NAME;
                    data.Price = 123M;
                    await TestContext.Out.WriteLineAsync("Executing test");
                    await sut.AddMinimumProductAsync(data);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    Product? p =context.Products.FirstOrDefault(p => p.Name == TEST_NAME && p.Sku == TEST_NAME);
                    p.Should().NotBeNull();

                }
            }
        }

        [Test, Description("Test an instance is returned")]
        public async Task DeleteAsync_MarksTheProductDeleted_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up and test");
            using (IServiceScope serviceScope =
                   _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>())
                {
                    Product p = context.Products.First();
                    p.Should().NotBeNull();
                    p.Deleted = true;
                    await context.SaveChangesAsync();

                    ProductRepo sut = new(context);

                    await TestContext.Out.WriteLineAsync("Executing test");
                    await sut.DeleteAsync(p.Id);

                    await TestContext.Out.WriteLineAsync("Examining results");
                    p = context.Products.First(pr=>pr.Id == p.Id);
                    p.Should().NotBeNull();
                    p.Deleted.Should().BeTrue();

                }
            }
        }

    }
}