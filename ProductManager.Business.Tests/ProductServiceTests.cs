using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Business.Tests.DataFactories;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Repos;

namespace ProductManager.Business.Tests
{
    [TestFixture, Description("Tests of the ProductService")]
    public class ProductServiceTests
    {
        [Test, Description("Test required logger object")]
        public void Constructor_RequiredILogger_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductService(null!,
                new Mock<IProductRepo>().Object,
                new Mock<IProductOptionRepo>().Object,
                new Mock<IProductCharacteristicRepo>().Object));
        }

        [Test, Description("Test required product repo object")]
        public void Constructor_RequiredIProductRepo_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductService(
                new Mock<ILogger<ProductService>>().Object,
                null!,
                new Mock<IProductOptionRepo>().Object,
                new Mock<IProductCharacteristicRepo>().Object));
        }

        [Test, Description("Test required product option repo object")]
        public void Constructor_RequiredIProductOptionRepo_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductService(
                new Mock<ILogger<ProductService>>().Object,
                new Mock<IProductRepo>().Object,
                null!,
                new Mock<IProductCharacteristicRepo>().Object));
        }

        [Test, Description("Test required product characteristic repo object")]
        public void Constructor_RequiredIProductCharacteristicRepo_Fail()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ProductService(
                new Mock<ILogger<ProductService>>().Object,
                new Mock<IProductRepo>().Object,
                new Mock<IProductOptionRepo>().Object,
                null!));
        }

        [Test, Description("Test required objects")]
        public async Task Constructor_RequiredObjects_Success()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            await TestContext.Out.WriteLineAsync("Executing test");
            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            await TestContext.Out.WriteLineAsync("Examining results");
            sut.Should().NotBeNull();
        }

        [Test, Description("The get short product should return data and pass its parameters down to the repo")]
        public async Task GetShortProduct_ReturnsData_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            IEnumerable<IProduct> data = ProductFactory.BuildShortProductList();
            productRepo.Setup(m=>m.FindPagedProductRecordsAsync(It.IsAny<Dictionary<string, IFilterMetaData[]>>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            Dictionary<string,IFilterMetaData[]> filterParameter = new();
            const int PAGE = 1;
            const int PAGE_SIZE = 10;

            await TestContext.Out.WriteLineAsync("Executing test");
            IEnumerable<IProduct> results = await sut.GetProductsAsync(filterParameter, PAGE, PAGE_SIZE,TestContext.CurrentContext.CancellationToken);
            
            await TestContext.Out.WriteLineAsync("Examining results");
            results.Should().NotBeNull();
            results.Should().NotBeEmpty();
            results.Should().HaveCount(2);
            productRepo.Verify(m=>m.FindPagedProductRecordsAsync(filterParameter,PAGE, 
                PAGE_SIZE,It.IsAny<CancellationToken>()));
        }

        [Test, Description("The get product count should call the repo to get the number of products")]
        public async Task GetProductCountAsync_CallsRepoProductCount_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            productRepo.Setup(m => m.GetProductCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(25);

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            long results = await sut.GetProductCountAsync(TestContext.CurrentContext.CancellationToken);
            results.Should().BeGreaterOrEqualTo(1, "The mock is set up to return 25");
            productRepo.Verify();
        }

        [Test, Description("The get product count should call the repo to get the number of products")]
        public async Task DeleteProductAsync_CallsRepoProductDelete_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            productRepo.Setup(m => m.DeleteAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>()));

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            await sut.DeleteProductAsync(Guid.NewGuid(), TestContext.CurrentContext.CancellationToken);
            productRepo.Verify();
        }

        [Test, Description("Creating a minimum viable product should populate defaults and delegate to the repo")]
        public async Task CreateMinimumViableProductAsync_CallsRepoAddMinimumProduct_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            Mock<IProduct> product = new();
            product.SetupAllProperties();
            productRepo.Setup(m => m.CreateInstance()).Returns(product.Object);

            Guid expectedId = Guid.NewGuid();
            productRepo.Setup(m => m.AddMinimumProductAsync(It.IsAny<IProduct>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            const string SKU = "SKU-1";
            const string NAME = "Widget";
            const string SHORT_DESCRIPTION = "A widget";
            const decimal PRICE = 9.99m;

            await TestContext.Out.WriteLineAsync("Executing test");
            Guid result = await sut.CreateMinimumViableProductAsync(SKU, NAME, SHORT_DESCRIPTION, PRICE, TestContext.CurrentContext.CancellationToken);

            await TestContext.Out.WriteLineAsync("Examining results");
            result.Should().Be(expectedId);
            product.Object.Sku.Should().Be(SKU);
            product.Object.Name.Should().Be(NAME);
            product.Object.ShortDescription.Should().Be(SHORT_DESCRIPTION);
            product.Object.Price.Should().Be(PRICE);
            product.Object.Description.Should().Be("Currently unavailable");
            productRepo.Verify(m => m.AddMinimumProductAsync(product.Object, It.IsAny<CancellationToken>()));
        }

        [Test, Description("Getting a product should delegate to the repo")]
        public async Task GetProductAsync_CallsRepoGetProduct_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            Guid productId = Guid.NewGuid();
            Mock<IFullProduct> fullProduct = new();
            productRepo.Setup(m => m.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fullProduct.Object);

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            IFullProduct? result = await sut.GetProductAsync(productId, TestContext.CurrentContext.CancellationToken);

            await TestContext.Out.WriteLineAsync("Examining results");
            result.Should().Be(fullProduct.Object);
            productRepo.Verify(m => m.GetProductAsync(productId, TestContext.CurrentContext.CancellationToken));
        }

        [Test, Description("Adding a product option with an invalid product ID should throw")]
        public void AddProductOptionAsync_InvalidProductId_ThrowsArgumentException()
        {
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() =>
                sut.AddProductOptionAsync(Guid.Empty, Guid.NewGuid(), 5m));
        }

        [Test, Description("Adding a product option with an invalid option ID should throw")]
        public void AddProductOptionAsync_InvalidOptionId_ThrowsArgumentException()
        {
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() =>
                sut.AddProductOptionAsync(Guid.NewGuid(), Guid.Empty, 5m));
        }

        [Test, Description("Adding a product option with valid IDs and a positive price override should use that price")]
        public async Task AddProductOptionAsync_PositivePriceOverride_UsesProvidedPrice()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            Mock<IProductOption> productOption = new();
            productOption.SetupAllProperties();
            productOptionRepo.Setup(m => m.CreateInstance()).Returns(productOption.Object);

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            Guid productId = Guid.NewGuid();
            Guid optionId = Guid.NewGuid();
            const decimal PRICE_OVERRIDE = 12.50m;

            await TestContext.Out.WriteLineAsync("Executing test");
            bool result = await sut.AddProductOptionAsync(productId, optionId, PRICE_OVERRIDE, TestContext.CurrentContext.CancellationToken);

            await TestContext.Out.WriteLineAsync("Examining results");
            result.Should().BeTrue();
            productOption.Object.ProductId.Should().Be(productId);
            productOption.Object.OptionId.Should().Be(optionId);
            productOption.Object.Price.Should().Be(PRICE_OVERRIDE);
            productOptionRepo.Verify(m => m.AddProductOptionAsync(productOption.Object, TestContext.CurrentContext.CancellationToken));
        }

        [Test, Description("Adding a product option with a zero or negative price override should default to zero")]
        public async Task AddProductOptionAsync_NonPositivePriceOverride_DefaultsToZero()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            Mock<IProductOption> productOption = new();
            productOption.SetupAllProperties();
            productOptionRepo.Setup(m => m.CreateInstance()).Returns(productOption.Object);

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            bool result = await sut.AddProductOptionAsync(Guid.NewGuid(), Guid.NewGuid(), -3m, TestContext.CurrentContext.CancellationToken);

            await TestContext.Out.WriteLineAsync("Examining results");
            result.Should().BeTrue();
            productOption.Object.Price.Should().Be(decimal.Zero);
        }

        [Test, Description("Deleting a product option should delegate to the option repo")]
        public async Task DeleteProductOptionAsync_CallsOptionRepoDelete_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            Guid optionRecordId = Guid.NewGuid();
            productOptionRepo.Setup(m => m.DeleteAsync(optionRecordId, It.IsAny<CancellationToken>()));

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            await sut.DeleteProductOptionAsync(optionRecordId, TestContext.CurrentContext.CancellationToken);

            await TestContext.Out.WriteLineAsync("Examining results");
            productOptionRepo.Verify(m => m.DeleteAsync(optionRecordId, TestContext.CurrentContext.CancellationToken));
        }

        [Test, Description("Listing product characteristics should delegate to the characteristic repo")]
        public async Task ListProductCharacteristicsAsync_CallsCharacteristicRepoList_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            Guid productId = Guid.NewGuid();
            Mock<IProductCharacteristic> characteristic = new();
            IEnumerable<IProductCharacteristic> data = new List<IProductCharacteristic> { characteristic.Object };
            productCharacteristicRepo.Setup(m => m.ListProductCharacteristicsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            IEnumerable<IProductCharacteristic> results = await sut.ListProductCharacteristicsAsync(productId,TestContext.CurrentContext.CancellationToken);

            await TestContext.Out.WriteLineAsync("Examining results");
            results.Should().BeEquivalentTo(data);
            productCharacteristicRepo.Verify(m => m.ListProductCharacteristicsAsync(productId, TestContext.CurrentContext.CancellationToken));
        }

        [Test, Description("Adding a product characteristic with an invalid product ID should throw")]
        public void AddProductCharacteristic_InvalidProductId_ThrowsArgumentException()
        {
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() =>
                sut.AddProductCharacteristic(Guid.Empty, "Color", "Red"));
        }

        [Test, Description("Adding a product characteristic with a null or empty name should throw")]
        public void AddProductCharacteristic_InvalidName_ThrowsArgumentException()
        {
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() =>
                sut.AddProductCharacteristic(Guid.NewGuid(), string.Empty, "Red"));
        }

        [Test, Description("Adding a product characteristic with a null or empty value should throw")]
        public void AddProductCharacteristic_InvalidValue_ThrowsArgumentException()
        {
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() =>
                sut.AddProductCharacteristic(Guid.NewGuid(), "Color", string.Empty));
        }

        [Test, Description("Adding a product characteristic with valid arguments should populate and delegate to the repo")]
        public async Task AddProductCharacteristic_ValidArguments_CallsCharacteristicRepoAdd_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            Mock<IProductCharacteristic> characteristic = new();
            characteristic.SetupAllProperties();
            productCharacteristicRepo.Setup(m => m.CreateInstance()).Returns(characteristic.Object);

            Guid expectedId = Guid.NewGuid();
            productCharacteristicRepo.Setup(m => m.AddProductCharacteristicAsync(It.IsAny<IProductCharacteristic>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            Guid productId = Guid.NewGuid();
            const string NAME = "Color";
            const string VALUE = "Red";

            await TestContext.Out.WriteLineAsync("Executing test");
            Guid result = await sut.AddProductCharacteristic(productId, NAME, VALUE, TestContext.CurrentContext.CancellationToken);

            await TestContext.Out.WriteLineAsync("Examining results");
            result.Should().Be(expectedId);
            characteristic.Object.ProductId.Should().Be(productId);
            characteristic.Object.Name.Should().Be(NAME);
            characteristic.Object.CharacteristicValue.Should().Be(VALUE);
            productCharacteristicRepo.Verify(m => m.AddProductCharacteristicAsync(characteristic.Object, TestContext.CurrentContext.CancellationToken));
        }

        [Test, Description("Deleting a product characteristic should delegate to the characteristic repo")]
        public async Task DeleteProductCharacteristicAsync_CallsCharacteristicRepoDelete_Successfully()
        {
            await TestContext.Out.WriteLineAsync("Setting up test");
            Mock<ILogger<ProductService>> logger = new();
            Mock<IProductRepo> productRepo = new();
            Mock<IProductOptionRepo> productOptionRepo = new();
            Mock<IProductCharacteristicRepo> productCharacteristicRepo = new();

            Guid characteristicId = Guid.NewGuid();
            productCharacteristicRepo.Setup(m => m.DeleteAsync(characteristicId, It.IsAny<CancellationToken>()));

            ProductService sut = new(logger.Object, productRepo.Object, productOptionRepo.Object, productCharacteristicRepo.Object);

            await TestContext.Out.WriteLineAsync("Executing test");
            await sut.DeleteProductCharacteristicAsync(characteristicId, TestContext.CurrentContext.CancellationToken);

            await TestContext.Out.WriteLineAsync("Examining results");
            productCharacteristicRepo.Verify(m => m.DeleteAsync(characteristicId, TestContext.CurrentContext.CancellationToken));
        }
    }
}