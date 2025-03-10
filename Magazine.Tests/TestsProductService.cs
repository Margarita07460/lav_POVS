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
    }
}