using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;
using Magazine.WebApi.Controllers;
using Magazine.Core.Services;
using Magazine.Core.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Magazine.Tests
{
    [TestFixture]
    public class TestsProductController
    {
        private Mock<IProductService> _mockProductService;
        private ProductController _productController;

        [SetUp]
        public void Setup()
        {
            _mockProductService = new Mock<IProductService>();
            _productController = new ProductController(_mockProductService.Object);
        }

        [Test]
        public async Task AddProduct_ReturnsOkResult()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            _mockProductService.Setup(service => service.Add(It.IsAny<Product>()))
                .ReturnsAsync(product);

            // Act
            var result = await _productController.Add(product);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task RemoveProduct_ReturnsOkResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductService.Setup(service => service.Remove(productId))
                .ReturnsAsync(new Product { Id = productId, Name = "Test Product", Price = 100.0M });

            // Act
            var result = await _productController.Remove(productId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
        [Test]
        public async Task AddProduct_ReturnsCorrectProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.0M };
            _mockProductService.Setup(service => service.Add(It.IsAny<Product>()))
                .ReturnsAsync(product);

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
        }
        [Test]
        public async Task EditProduct_ReturnsOkResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updatedProduct = new Product { Name = "Updated Product", Price = 200.0M };
            _mockProductService.Setup(service => service.Edit(productId, It.IsAny<Product>()))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _productController.Edit(productId, updatedProduct);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
        [Test]
        public async Task SearchProduct_ReturnsOkResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Price = 100.0M };
            _mockProductService.Setup(service => service.Search(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _productController.Search(productId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}
