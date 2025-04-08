using NUnit.Framework;
using Moq;
using Magazine.Core.Services;
using Magazine.Core.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Magazine.WebApi.Services;

namespace Magazine.Tests
{
    [TestFixture]
    public class TestsProductService
    {
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config["DataBaseFilePath"]).Returns("test_database.txt");

            var mockLogger = new Mock<ILogger<ProductService>>();

            _productService = new ProductService(mockConfiguration.Object, mockLogger.Object);
        }

        [Test]
        public async Task AddProduct_ShouldAddToMemoryAndFile()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };

            // Act
            var addedProduct = await _productService.Add(product);

            // Assert
            Assert.NotNull(addedProduct);
            Assert.AreEqual("Test Product", addedProduct.Name);
        }
        [Test]
        public async Task RemoveProduct_ShouldRemoveFromMemoryAndFile()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            var addedProduct = await _productService.Add(product);

            // Act
            var removedProduct = await _productService.Remove(addedProduct.Id);

            // Assert
            Assert.NotNull(removedProduct);
            Assert.AreEqual(addedProduct.Id, removedProduct.Id);
        }
        [Test]
        public async Task EditProduct_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            var addedProduct = await _productService.Add(product);

            var updatedProduct = new Product { Name = "Updated Product", Price = 200.0M };

            // Act
            var editedProduct = await _productService.Edit(addedProduct.Id, updatedProduct);

            // Assert
            Assert.NotNull(editedProduct);
            Assert.AreEqual("Updated Product", editedProduct.Name);
            Assert.AreEqual(200.0M, editedProduct.Price);
        }
        [Test]
        public async Task SearchProduct_ShouldReturnProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            var addedProduct = await _productService.Add(product);

            // Act
            var foundProduct = await _productService.Search(addedProduct.Id);

            // Assert
            Assert.NotNull(foundProduct);
            Assert.AreEqual(addedProduct.Id, foundProduct.Id);
        }
        [Test]
        public async Task RemoveProduct_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _productService.Remove(nonExistentId);

            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task SearchProduct_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _productService.Search(nonExistentId);

            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task AddProduct_ShouldPersistToFile()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };

            // Act
            var addedProduct = await _productService.Add(product);
            var foundProduct = await _productService.Search(addedProduct.Id);

            // Assert
            Assert.NotNull(foundProduct);
            Assert.AreEqual(addedProduct.Id, foundProduct.Id);
            Assert.AreEqual("Test Product", foundProduct.Name);
            Assert.AreEqual(100.0M, foundProduct.Price);
        }
    }
}