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

            // Создаем пустой файл, если он не существует
            if (!System.IO.File.Exists("test_database.txt"))
            {
                System.IO.File.WriteAllText("test_database.txt", "{}");
            }


            // Создание реального сервиса
            _productService = new ProductService(mockConfiguration.Object, mockLogger.Object);

            // Создание контроллера с реальным сервисом
            _productController = new ProductController(_productService);
        }

        [Test]
        public async Task AddProduct_ShouldReturnCorrectProduct()
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
            Assert.AreEqual(product.Name, returnedProduct.Name);
            Assert.AreEqual(product.Price, returnedProduct.Price);
            Assert.AreNotEqual(Guid.Empty, returnedProduct.Id); // Проверка, что Id был присвоен
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

        [Test]
        public async Task RemoveProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _productController.Remove(nonExistentId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task SearchProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _productController.Search(nonExistentId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        [TestCase("Product 1", 100.0)]
        [TestCase("Product 2", 200.0)]
        [TestCase("Product 3", 300.0)]
        public async Task AddProduct_ShouldReturnCorrectProduct_ForDifferentInputs(string name, decimal price)
        {
            // Arrange
            var product = new Product { Name = name, Price = price };

            // Act
            var result = await _productController.Add(product);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var returnedProduct = okResult.Value as Product;
            Assert.NotNull(returnedProduct);
            Assert.AreEqual(name, returnedProduct.Name);
            Assert.AreEqual(price, returnedProduct.Price);
        }

    }
}