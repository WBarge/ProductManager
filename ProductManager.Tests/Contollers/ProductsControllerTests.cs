using System.Net;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductManager.Service.Controllers;
using Moq;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Services;
using ProductManager.Service.Models.Request;
using ProductManager.Service.Tests.DataFactories;

namespace ProductManager.Service.Tests.Contollers
{
    [TestFixture, Description("Tests of the ProductsController")]
    public class ProductsControllerTests
    {
        [Test, Description("Test required logger object")]
        public void Constructor_RequiredILogger_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductsController(null!,new Mock<IProductService>().Object));
        }

        [Test, Description("Test required service object")]
        public void Constructor_RequiredIProductService_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductsController(new Mock<ILogger<ProductsController>>().Object  ,null!));
        }

        [Test, Description("Test required objects")]
        public async Task Constructor_RequiredObjects_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductsController>> logger = new();
            Mock<IProductService> productService = new();

            await TestContext.Out.WriteLineAsync("Executing test");
            ProductsController sut = new(logger.Object, productService.Object);

            await TestContext.Out.WriteLineAsync("Examining results");
            sut.Should().NotBeNull();
        }

        [Test,Description("Simulate a get with no parameters")]
        public async Task GetProducts_Successfully_ReturnsData()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductsController>> logger = new();
            Mock<IProductService> productService = new();

            IEnumerable<IProduct> data = ProductFactory.BuildProductList();
            productService.Setup(s => s.GetProductsAsync(It.IsAny<Dictionary<string, IFilterMetaData[]>>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);
            productService.Setup(s=>s.GetProductCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(data.Count());

            ProductsController sut = new(logger.Object, productService.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            IActionResult result = await sut.GetProducts(null!);

            await TestContext.Out.WriteLineAsync("Examining results");
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult? castedResult = result as OkObjectResult;
            castedResult.Should().NotBeNull();
            castedResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            object? resultData = castedResult.Value;
            Type t = resultData!.GetType();
            t.Name.Should().Contain("AnonymousType");
            PropertyInfo[] f = t.GetProperties();
            f.Length.Should().Be(2);
            IEnumerable<IShortProduct>? returnedData = (IEnumerable<IShortProduct>?)f[0].GetValue(resultData);
            returnedData.Should().NotBeNull();
            returnedData!.Count().Should().Be(2);
            long totalRecordSize = (long)(f[1].GetValue(resultData) ?? 0);
            totalRecordSize.Should().BeGreaterThan(0);
        }

        [Test,Description("Simulate a get with the paging information set")]
        public async Task GetProductsPaged_Successfully_Returns()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductsController>> logger = new();
            Mock<IProductService> productService = new();

            IEnumerable<IProduct> data = ProductFactory.BuildProductList();
            productService.Setup(s => s.GetProductsAsync(It.IsAny<Dictionary<string, IFilterMetaData[]>>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);

            ProductsController sut = new(logger.Object, productService.Object);

            ProductListRequest request = new()
            {
                Page = 1,
                PageSize = 1,
            };

            await TestContext.Out.WriteLineAsync("Executing test");
            IActionResult result = await sut.GetProducts(request);

            await TestContext.Out.WriteLineAsync("Examining results");
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult? castedResult = result as OkObjectResult;
            castedResult.Should().NotBeNull();
            castedResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            productService.Verify(s=>s.GetProductsAsync(It.IsAny<Dictionary<string, IFilterMetaData[]>>(),
                1, 1, It.IsAny<CancellationToken>()));
        }

        [Test, Description("Ensure QuickAdd calls the product service")]
        public async Task QuickAdd_Successfully_Returns()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductsController>> logger = new();
            Mock<IProductService> productService = new();

            productService.Setup(s => s.CreateMinimumViableProductAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<CancellationToken>()));

            ProductsController sut = new(logger.Object, productService.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            QuickProductRequest request = new()
            {
                Sku = string.Empty,
                Name = string.Empty,
                ShortDescription = string.Empty,
                Price = 0M
            };
            IActionResult result = await sut.QuickAdd(request);
            await TestContext.Out.WriteLineAsync("Examining results");
            productService.Verify();
        }

        [Test, Description("Ensure delete calls the product service")]
        public async Task DeleteProduct_Successfully_Returns()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductsController>> logger = new();
            Mock<IProductService> productService = new();

            productService.Setup(s => s.DeleteProductAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()));

            ProductsController sut = new(logger.Object, productService.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            IActionResult result = await sut.DeleteProduct(Guid.NewGuid());
            await TestContext.Out.WriteLineAsync("Examining results");
            productService.Verify();
        }
    }
}