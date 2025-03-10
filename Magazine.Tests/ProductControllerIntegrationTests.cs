using NUnit.Framework;
using Magazine.WebApi.Controllers;
using Magazine.Core.Services;
using Magazine.Core.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Magazine.WebApi.Services;

namespace Magazine.Tests
{
    [TestFixture]
    public class ProductControllerIntegrationTests
    {
        private ProductController _productController;
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            // Настройка конфигурации
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config["DataBaseFilePath"]).Returns("test_database.txt");

            // Настройка логгера
            var mockLogger = new Mock<ILogger<ProductService>>();

            // Создание реального сервиса
            _productService = new ProductService(mockConfiguration.Object, mockLogger.Object);

            // Создание контроллера с реальным сервисом
            _productController = new ProductController(_productService);
        }

        [Test]
        public async Task AddProduct_ShouldReturnOkResult()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };

            // Act
            var result = await _productController.Add(product);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var returnedProduct = okResult.Value as Product;
            Assert.NotNull(returnedProduct);
            Assert.AreEqual("Test Product", returnedProduct.Name);
        }

        [Test]
        public async Task RemoveProduct_ShouldReturnOkResult()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            var addedProduct = await _productService.Add(product);

            // Act
            var result = await _productController.Remove(addedProduct.Id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var removedProduct = okResult.Value as Product;
            Assert.NotNull(removedProduct);
            Assert.AreEqual(addedProduct.Id, removedProduct.Id);
        }

        [Test]
        public async Task EditProduct_ShouldReturnOkResult()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            var addedProduct = await _productService.Add(product);

            var updatedProduct = new Product { Name = "Updated Product", Price = 200.0M };

            // Act
            var result = await _productController.Edit(addedProduct.Id, updatedProduct);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var editedProduct = okResult.Value as Product;
            Assert.NotNull(editedProduct);
            Assert.AreEqual("Updated Product", editedProduct.Name);
            Assert.AreEqual(200.0M, editedProduct.Price);
        }

        [Test]
        public async Task SearchProduct_ShouldReturnOkResult()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            var addedProduct = await _productService.Add(product);

            // Act
            var result = await _productController.Search(addedProduct.Id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var foundProduct = okResult.Value as Product;
            Assert.NotNull(foundProduct);
            Assert.AreEqual(addedProduct.Id, foundProduct.Id);
        }
    }
}