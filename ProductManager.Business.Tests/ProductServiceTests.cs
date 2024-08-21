using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Business.Tests.DataFactories;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Repos;
using ProductManager.Glue.Interfaces.Services;

namespace ProductManager.Business.Tests
{
    [TestFixture, Description("Tests of the ProductService")]
    public class ProductServiceTests
    {
        [Test, Description("Test required logger object")]
        public void Constructor_RequiredILogger_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductService(null!,new Mock<IProductRepo>().Object));
        }

        [Test, Description("Test required service object")]
        public void Constructor_RequiredIProductService_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductService(new Mock<ILogger<ProductService>>().Object  ,null!));
        }

        [Test, Description("Test required objects")]
        public async Task Constructor_RequiredObjects_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();

            await TestContext.Out.WriteLineAsync("Executing test");
            ProductService sut = new(logger.Object, productRepo.Object);

            await TestContext.Out.WriteLineAsync("Examining results");
            sut.Should().NotBeNull();
        }

        [Test, Description("The get short product should return data and pass its parameters down to the repo")]
        public async Task GetShortProduct_ReturnsData_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();

            IEnumerable<IProduct> data = ProductFactory.BuildShortProductList();
            productRepo.Setup(m=>m.FindPagedProductRecordsAsync(It.IsAny<Dictionary<string, IFilterMetaData[]>>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);

            ProductService sut = new(logger.Object, productRepo.Object);

            Dictionary<string,IFilterMetaData[]> filterParameter = new();
            const int page = 1;
            const int pageSize = 10;

            await TestContext.Out.WriteLineAsync("Executing test");
            IEnumerable<IShortProduct> results = await sut.GetProductsAsync(filterParameter, page, pageSize);
            
            await TestContext.Out.WriteLineAsync("Examining results");
            results.Should().NotBeNull();
            results.Should().NotBeEmpty();
            results.Should().HaveCount(2);
            productRepo.Verify(m=>m.FindPagedProductRecordsAsync(filterParameter,page, 
                pageSize,It.IsAny<CancellationToken>()));
        }

        [Test, Description("The get product count should call the repo to get the number of products")]
        public async Task GetProductCountAsync_CallsRepoProductCount_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();

            productRepo.Setup(m => m.GetProductCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(25);



            ProductService sut = new(logger.Object, productRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            long results = await sut.GetProductCountAsync();
            results.Should().BeGreaterOrEqualTo(1, "The mock is set up to return 25");
            productRepo.Verify();
        }

        [Test, Description("The get product count should call the repo to get the number of products")]
        public async Task DeleteProductAsync_CallsRepoProductDelete_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();

            productRepo.Setup(m => m.DeleteAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>()));



            ProductService sut = new(logger.Object, productRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            await sut.DeleteProductAsync(Guid.NewGuid());
            productRepo.Verify();
        }
    }
}