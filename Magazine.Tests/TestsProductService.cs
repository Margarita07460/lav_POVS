using NUnit.Framework;
using Moq;
using Magazine.Core.Services;
using Magazine.Core.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using Magazine.WebApi.Services;

namespace Magazine.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private ProductService _productService;
        private string _testDbPath;

        [SetUp]
        public void Setup()
        {
            // Создаем уникальный файл БД для каждого теста
            _testDbPath = $"test_db_{Guid.NewGuid()}.db";

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config["DataBaseFilePath"]).Returns(_testDbPath);

            var mockLogger = new Mock<ILogger<ProductService>>();

            _productService = new ProductService(mockConfiguration.Object, mockLogger.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            // Удаляем тестовую БД после каждого теста
            if (File.Exists(_testDbPath))
            {
                try { File.Delete(_testDbPath); }
                catch { /* Игнорируем ошибки удаления */ }
            }
        }

        [Test]
        public async Task AddProduct_ShouldAddProductToDatabase()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };

            // Act
            var result = await _productService.Add(product);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Test Product", result.Name);
            Assert.AreEqual(100.0M, result.Price);
            Assert.AreNotEqual(Guid.Empty, result.Id);
        }

        [Test]
        public async Task RemoveProduct_ShouldRemoveExistingProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            var addedProduct = await _productService.Add(product);

            // Act
            var removedProduct = await _productService.Remove(addedProduct.Id);
            var searchResult = await _productService.Search(addedProduct.Id);

            // Assert
            Assert.NotNull(removedProduct);
            Assert.AreEqual(addedProduct.Id, removedProduct.Id);
            Assert.IsNull(searchResult);
        }

        [Test]
        public async Task EditProduct_ShouldUpdateExistingProduct()
        {
            // Arrange
            var originalProduct = new Product { Name = "Original", Price = 100.0M };
            var addedProduct = await _productService.Add(originalProduct);

            var updatedProduct = new Product
            {
                Name = "Updated",
                Price = 200.0M,
                Definition = "New description",
                Image = "new_image.jpg"
            };

            // Act
            var result = await _productService.Edit(addedProduct.Id, updatedProduct);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Updated", result.Name);
            Assert.AreEqual(200.0M, result.Price);
            Assert.AreEqual("New description", result.Definition);
            Assert.AreEqual("new_image.jpg", result.Image);
        }

        [Test]
        public async Task SearchProduct_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            var addedProduct = await _productService.Add(product);

            // Act
            var result = await _productService.Search(addedProduct.Id);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedProduct.Id, result.Id);
        }

        [Test]
        public async Task RemoveProduct_ShouldReturnNull_WhenProductNotExists()
        {
            // Act
            var result = await _productService.Remove(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task SearchProduct_ShouldReturnNull_WhenProductNotExists()
        {
            // Act
            var result = await _productService.Search(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddProduct_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var product = new Product { Name = "", Price = 100.0M };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _productService.Add(product));
        }

        [Test]
        public async Task AddProduct_ShouldThrowException_WhenPriceIsNegative()
        {
            // Arrange
            var product = new Product { Name = "Test", Price = -100.0M };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _productService.Add(product));
        }
    }
}