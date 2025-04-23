using NUnit.Framework;
using Magazine.WebApi.Controllers;
using Magazine.Core.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Magazine.WebApi.Services;

namespace Magazine.Tests.Integration
{
    [TestFixture]
    public class ProductControllerIntegrationTests
    {
        private ProductController _productController;
        private ProductService _productService;
        private string _testDbPath;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _testDbPath = $"test_db_{Guid.NewGuid()}.db";

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            _configuration["DataBaseFilePath"] = _testDbPath;

            var logger = Mock.Of<ILogger<ProductService>>();
            _productService = new ProductService(_configuration, logger);
            _productController = new ProductController(_productService);
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(_testDbPath))
            {
                try { File.Delete(_testDbPath); }
                catch { /* Ignore */ }
            }
        }

        [Test]
        public async Task Database_ShouldBeCreatedWithCorrectSchema()
        {
            // Act
            var tableExists = await CheckTableExists("Products");
            var indexExists = await CheckIndexExists("idx_products_id");

            // Assert
            Assert.IsTrue(tableExists, "Таблица Products должна существовать");
            Assert.IsTrue(indexExists, "Индекс idx_products_id должен существовать");
        }

        [Test]
        public async Task AddProduct_ShouldPersistAllFieldsCorrectly()
        {
            // Arrange
            var product = new Product
            {
                Name = "Full Product",
                Price = 150.0M,
                Definition = "Test description",
                Image = "test.jpg"
            };

            // Act
            var result = await _productController.Add(product);
            var okResult = result as OkObjectResult;
            var addedProduct = okResult.Value as Product;
            var fromDb = await _productService.Search(addedProduct.Id);

            // Assert
            Assert.AreEqual(product.Name, fromDb.Name);
            Assert.AreEqual(product.Price, fromDb.Price);
            Assert.AreEqual(product.Definition, fromDb.Definition);
            Assert.AreEqual(product.Image, fromDb.Image);
        }



        [Test]
        public async Task ConcurrentAccess_ShouldHandleMultipleRequests()
        {
            // Arrange
            var product = new Product { Name = "Concurrent", Price = 100.0M };
            await _productController.Add(product);
            var addedProduct = (await _productService.Search(product.Id))!;

            // Act
            var tasks = Enumerable.Range(0, 10).Select(i =>
                Task.Run(async () =>
                {
                    var updated = new Product { Name = $"Updated {i}", Price = 100.0M + i };
                    await _productController.Edit(addedProduct.Id, updated);
                }));

            await Task.WhenAll(tasks);
            var finalProduct = await _productService.Search(addedProduct.Id);

            // Assert
            Assert.IsNotNull(finalProduct);
            Assert.That(finalProduct.Name, Does.StartWith("Updated "));
            Assert.GreaterOrEqual(finalProduct.Price, 100.0M);
        }

        private async Task<bool> CheckTableExists(string tableName)
        {
            using var connection = new SqliteConnection($"Data Source={_testDbPath}");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT name FROM sqlite_master WHERE type='table' AND name=@name";
            command.Parameters.AddWithValue("@name", tableName);

            return await command.ExecuteScalarAsync() != null;
        }

        private async Task<bool> CheckIndexExists(string indexName)
        {
            using var connection = new SqliteConnection($"Data Source={_testDbPath}");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT name FROM sqlite_master WHERE type='index' AND name=@name";
            command.Parameters.AddWithValue("@name", indexName);

            return await command.ExecuteScalarAsync() != null;
        }
    }
}