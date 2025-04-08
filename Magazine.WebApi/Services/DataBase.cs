using Microsoft.Data.Sqlite;
using Magazine.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magazine.WebApi.Services
{
    public class DataBase : IDisposable
    {
        private readonly string _connectionString;

        private const string CREATE_TABLE = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Definition TEXT,
                Price REAL NOT NULL,
                Image TEXT
            )";

        private const string CREATE_INDEX = @"
            CREATE INDEX IF NOT EXISTS idx_products_id ON Products(Id)";

        private const string SELECT_ALL_PRODUCTS = @"
            SELECT Id, Name, Definition, Price, Image
            FROM Products";

        private const string SELECT_PRODUCT_BY_ID = @"
            SELECT Id, Name, Definition, Price, Image
            FROM Products
            WHERE Id = @Id";

        private const string INSERT_PRODUCT = @"
            INSERT INTO Products (Id, Name, Definition, Price, Image)
            VALUES (@Id, @Name, @Definition, @Price, @Image)";

        private const string UPDATE_PRODUCT = @"
            UPDATE Products
            SET Name = @Name, Definition = @Definition, Price = @Price, Image = @Image
            WHERE Id = @Id";

        private const string DELETE_PRODUCT = @"
            DELETE FROM Products
            WHERE Id = @Id";

        public DataBase(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase().GetAwaiter().GetResult();
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        private async Task InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using (var command = new SqliteCommand(CREATE_TABLE, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            using (var command = new SqliteCommand(CREATE_INDEX, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Product> AddProduct(Product product)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqliteCommand(INSERT_PRODUCT, connection);
            command.Parameters.AddWithValue("@Id", product.Id.ToString());
            command.Parameters.AddWithValue("@Name", product.Name ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Definition", product.Definition ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Image", product.Image ?? (object)DBNull.Value);
            await command.ExecuteNonQueryAsync();
            return product;
        }

        public async Task<Product?> GetProduct(Guid id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqliteCommand(SELECT_PRODUCT_BY_ID, connection);
            command.Parameters.AddWithValue("@Id", id.ToString());
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Product
                {
                    Id = Guid.Parse(reader.GetString(0)),
                    Name = reader.GetString(1),
                    Definition = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Image = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                };
            }
            return null;
        }

        public async Task<Product?> UpdateProduct(Guid id, Product product)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqliteCommand(UPDATE_PRODUCT, connection);
            command.Parameters.AddWithValue("@Id", id.ToString());
            command.Parameters.AddWithValue("@Name", product.Name ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Definition", product.Definition ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Image", product.Image ?? (object)DBNull.Value);
            int rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                return await GetProduct(id);
            }
            return null;
        }

        public async Task<Product?> DeleteProduct(Guid id)
        {
            var product = await GetProduct(id);
            if (product == null) return null;
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqliteCommand(DELETE_PRODUCT, connection);
            command.Parameters.AddWithValue("@Id", id.ToString());
            await command.ExecuteNonQueryAsync();
            return product;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = new List<Product>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqliteCommand(SELECT_ALL_PRODUCTS, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = Guid.Parse(reader.GetString(0)),
                    Name = reader.GetString(1),
                    Definition = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Image = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }
            return products;
        }

        public void Dispose()
        {
        }
    }
}