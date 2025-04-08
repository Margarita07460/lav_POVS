using NUnit.Framework;
using Magazine.Core.Models;
using Magazine.WebApi.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Magazine.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        private string _testDbPath;
        private DataBase _database;

        [SetUp]
        public void Setup()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_db_{Guid.NewGuid()}.db");
            _database = new DataBase(_testDbPath);
        }

        [TearDown]
        public void Cleanup()
        {
            _database.Dispose();
            if (File.Exists(_testDbPath))
            {
                try { File.Delete(_testDbPath); }
                catch { /* Ignore */ }
            }
        }

        [Test]
        public async Task DatabaseInitialization_ShouldCreateProductsTable()
        {
            // Act
            bool tableExists = await CheckTableExists("Products");

            // Assert
            Assert.IsTrue(tableExists, "Таблица Products должна быть создана");
        }

        [Test]
        public async Task DatabaseInitialization_ShouldCreateIdIndex()
        {
            // Act
            bool indexExists = await CheckIndexExists("idx_products_id");

            // Assert
            Assert.IsTrue(indexExists, "Индекс idx_products_id должен быть создан");
        }

        [Test]
        public async Task AddProduct_ShouldUseParameterizedQuery()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test'; DROP TABLE Products;--",
                Price = 9.99m,
                Definition = "Test Description",
                Image = "test.jpg"
            };

            // Act
            var result = await _database.AddProduct(product);
            var retrieved = await _database.GetProduct(product.Id);

            // Assert (если не упало с SQL-инъекцией, значит параметризация работает)
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(product.Name, retrieved.Name);
        }

        [Test]
        public async Task GetProduct_ShouldExecuteCorrectSelectQuery()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Price = 10.50m
            };
            await _database.AddProduct(product);

            // Act
            var result = await _database.GetProduct(product.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Id, result.Id);
            Assert.AreEqual(product.Name, result.Name);
            Assert.AreEqual(product.Price, result.Price);
        }

        [Test]
        public async Task UpdateProduct_ShouldModifyRecordWithCorrectQuery()
        {
            // Arrange
            var original = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Original",
                Price = 10.00m
            };
            await _database.AddProduct(original);

            var updated = new Product
            {
                Id = original.Id,
                Name = "Updated",
                Price = 20.00m,
                Definition = "New Description"
            };

            // Act
            var result = await _database.UpdateProduct(original.Id, updated);

            // Assert
            Assert.IsNotNull(result);
            var fromDb = await _database.GetProduct(original.Id);
            Assert.AreEqual("Updated", fromDb.Name);
            Assert.AreEqual(20.00m, fromDb.Price);
            Assert.AreEqual("New Description", fromDb.Definition);
        }

        [Test]
        public async Task DeleteProduct_ShouldRemoveRecordWithCorrectQuery()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "To Delete",
                Price = 15.99m
            };
            await _database.AddProduct(product);

            // Act
            var deleted = await _database.DeleteProduct(product.Id);
            var result = await _database.GetProduct(product.Id);

            // Assert
            Assert.IsNotNull(deleted);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllProducts_ShouldReturnCorrectData()
        {
            // Arrange
            var product1 = new Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 10m };
            var product2 = new Product { Id = Guid.NewGuid(), Name = "Product 2", Price = 20m };
            await _database.AddProduct(product1);
            await _database.AddProduct(product2);

            // Act
            var products = (await _database.GetAllProducts()).OrderBy(p => p.Name).ToList();

            // Assert
            Assert.AreEqual(2, products.Count);
            Assert.AreEqual("Product 1", products[0].Name);
            Assert.AreEqual("Product 2", products[1].Name);
        }

        [Test]
        public async Task AddProduct_ShouldHandleNullOptionalFields()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Price = 10.00m,
                Definition = null,
                Image = null
            };

            // Act
            var result = await _database.AddProduct(product);
            var retrieved = await _database.GetProduct(product.Id);

            // Assert
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(string.Empty, retrieved.Definition);
            Assert.AreEqual(string.Empty, retrieved.Image);
        }

        private async Task<bool> CheckTableExists(string tableName)
        {
            using var connection = new SqliteConnection($"Data Source={_testDbPath}");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT name FROM sqlite_master WHERE type='table' AND name=@name";
            command.Parameters.AddWithValue("@name", tableName);

            var result = await command.ExecuteScalarAsync();
            return result != null;
        }

        private async Task<bool> CheckIndexExists(string indexName)
        {
            using var connection = new SqliteConnection($"Data Source={_testDbPath}");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT name FROM sqlite_master WHERE type='index' AND name=@name";
            command.Parameters.AddWithValue("@name", indexName);

            var result = await command.ExecuteScalarAsync();
            return result != null;
        }
    }
}