﻿using System.Net;
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
            Assert.Throws<ArgumentNullException>(() => new ProductsController(null,new Mock<IProductService>().Object));
        }

        [Test, Description("Test required service object")]
        public void Constructor_RequiredIProductService_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductsController(new Mock<ILogger<ProductsController>>().Object  ,null));
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

            IEnumerable<IShortProduct> data = ProductFactory.BuildShortProductList();
            productService.Setup(s => s.GetShortProductsAsync(It.IsAny<Dictionary<string, IFilterMetaData[]>>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);

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
            int totalRecordSize = (int)(f[1].GetValue(resultData) ?? 0);
            totalRecordSize.Should().BeGreaterThan(0);
        }

        [Test,Description("Simulate a get with the paging information set")]
        public async Task GetProductsPaged_Successfully_Returns()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductsController>> logger = new();
            Mock<IProductService> productService = new();

            IEnumerable<IShortProduct> data = ProductFactory.BuildShortProductList();
            productService.Setup(s => s.GetShortProductsAsync(It.IsAny<Dictionary<string, IFilterMetaData[]>>(),
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
            productService.Verify(s=>s.GetShortProductsAsync(It.IsAny<Dictionary<string, IFilterMetaData[]>>(),
                1, 1, It.IsAny<CancellationToken>()));
        }
    }
}