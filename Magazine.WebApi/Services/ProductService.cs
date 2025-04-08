using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Magazine.Core.Models;
using Magazine.Core.Services;

namespace Magazine.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly DataBase _database;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IConfiguration configuration, ILogger<ProductService> logger)
        {
            var dbPath = configuration["DataBaseFilePath"] ?? "products.db";
            _database = new DataBase(dbPath);
            _logger = logger;
        }

        public async Task<Product> Add(Product product)
        {
            if (string.IsNullOrEmpty(product.Name) || product.Price < 0)
            {
                _logger.LogWarning("Попытка добавить продукт с некорректными данными: Name={Name}, Price={Price}", product.Name, product.Price);
                throw new ArgumentException("Name cannot be empty and Price must be non-negative.");
            }

            product.Id = Guid.NewGuid();
            var result = await _database.AddProduct(product);
            _logger.LogInformation("Продукт {Name} успешно добавлен с ID {Id}", product.Name, product.Id);
            return result;
        }

        public async Task<Product?> Remove(Guid id)
        {
            var result = await _database.DeleteProduct(id);
            if (result != null)
            {
                _logger.LogInformation("Продукт с ID {Id} успешно удален", id);
            }
            else
            {
                _logger.LogWarning("Продукт с ID {Id} не найден для удаления", id);
            }
            return result;
        }

        public async Task<Product?> Edit(Guid id, Product product)
        {
            if (string.IsNullOrEmpty(product.Name) || product.Price < 0)
            {
                _logger.LogWarning("Попытка обновить продукт с некорректными данными: Name={Name}, Price={Price}", product.Name, product.Price);
                throw new ArgumentException("Name cannot be empty and Price must be non-negative.");
            }

            var result = await _database.UpdateProduct(id, product);
            if (result != null)
            {
                _logger.LogInformation("Продукт с ID {Id} успешно обновлен", id);
            }
            else
            {
                _logger.LogWarning("Продукт с ID {Id} не найден для обновления", id);
            }
            return result;
        }

        public async Task<Product?> Search(Guid id)
        {
            var result = await _database.GetProduct(id);
            if (result == null)
            {
                _logger.LogWarning("Продукт с ID {Id} не найден", id);
            }
            return result;
        }

        public async Task PrintProducts()
        {
            var products = await _database.GetAllProducts();
            _logger.LogInformation("Текущее содержимое базы данных:");
            foreach (var product in products)
            {
                _logger.LogInformation("ID: {Id}, Название: {Name}, Цена: {Price}", product.Id, product.Name, product.Price);
            }
        }
    }
}