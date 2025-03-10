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
    }
}
