using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Repos;

namespace ProductManager.Business.Tests
{
    [TestFixture, Description("Unit tests for CharacteristicService")]
    public class CharacteristicServiceTests
    {
        private Mock<ILogger<CharacteristicService>> _loggerMock;
        private Mock<ICharacteristicRepo> _repoMock;
        private CharacteristicService _sut;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<CharacteristicService>>();
            _repoMock = new Mock<ICharacteristicRepo>();
            _sut = new CharacteristicService(_loggerMock.Object, _repoMock.Object);
        }

        [Test, Description("GetAllCharacteristicsAsync should return all characteristics")]
        public async Task GetAllCharacteristicsAsync_ReturnsAllCharacteristics()
        {
            // Arrange
            var mockCharacteristics = new List<IFullCharacteristic>
            {
                new Mock<IFullCharacteristic>().Object,
                new Mock<IFullCharacteristic>().Object
            };
            _repoMock.Setup(repo => repo.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCharacteristics);
            // Act
            var result = await _sut.GetAllCharacteristicsAsync();
            // Assert
            result.Should().BeEquivalentTo(mockCharacteristics);
            _repoMock.Verify(repo => repo.GetAll(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, Description("CreateCharacteristicAsync should create a new characteristic and return its ID")]
        public async Task CreateCharacteristicAsync_CreatesNewCharacteristic()
        {
            // Arrange
            var characteristicId = Guid.NewGuid();
            _repoMock.Setup(repo => repo.CreateInstance())
                .Returns(new Mock<ICharacteristic>().Object);
            _repoMock.Setup(repo => repo.Add(It.IsAny<ICharacteristic>(), It.IsAny<CancellationToken>()));
            // Act
            await _sut.CreateCharacteristicAsync("Test Characteristic");
            // Assert
            _repoMock.Verify(repo => repo.CreateInstance(), Times.Once);
            _repoMock.Verify(repo => repo.Add(It.IsAny<ICharacteristic>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, Description("DeleteCharacteristicAsync should delete the characteristic by ID")]
        public async Task DeleteCharacteristicAsync_DeletesCharacteristicById()
        {
            // Arrange
            var characteristicId = Guid.NewGuid();
            _repoMock.Setup(repo => repo.Delete(characteristicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            // Act
            await _sut.DeleteCharacteristicAsync(characteristicId);
            // Assert
            _repoMock.Verify(repo => repo.Delete(characteristicId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, Description("AddValueToCharacteristicAsync should add a value to the characteristic")]
        public async Task AddValueToCharacteristicAsync_AddsValueToCharacteristic()
        {
            // Arrange
            var characteristicId = Guid.NewGuid();
            var value = "Test Value";
            _repoMock.Setup(repo => repo.AddValue(It.IsAny<ICharacteristicValue>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            // Act
            await _sut.AddValueToCharacteristicAsync(characteristicId, value);
            // Assert
            _repoMock.Verify(repo => repo.AddValue(It.Is<ICharacteristicValue>(v => v.CharacteristicId == characteristicId && v.Value == value), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
