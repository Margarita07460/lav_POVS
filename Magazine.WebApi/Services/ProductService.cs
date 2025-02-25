using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Magazine.Core.Models;
using Magazine.Core.Services;

namespace Magazine.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly string _databaseFilePath;
        private readonly Dictionary<Guid, Product> _products;
        private readonly Mutex _fileMutex = new Mutex(); // Мьютекс для синхронизации записи в файл
        private readonly ILogger<ProductService> _logger;

        public ProductService(IConfiguration configuration, ILogger<ProductService> logger)
        {
            _configuration = configuration;
            _databaseFilePath = _configuration["DataBaseFilePath"];
            _logger = logger;

            // Проверка существования файла
            if (!File.Exists(_databaseFilePath))
            {
                File.WriteAllText(_databaseFilePath, "{}"); // Создаем пустой JSON-объект
                _logger.LogInformation("Файл базы данных не найден. Создан новый файл: {Path}", _databaseFilePath);
            }

            // Загрузка данных из файла
            _products = InitFromFile();
        }

        /// <summary>
        /// Загружает данные о продуктах из файла.
        /// </summary>
        /// <returns>Словарь продуктов, где ключ — идентификатор продукта.</returns>
        private Dictionary<Guid, Product> InitFromFile()
        {
            if (!File.Exists(_databaseFilePath))
            {
                _logger.LogInformation("Файл базы данных не найден. Создан пустой словарь продуктов.");
                return new Dictionary<Guid, Product>();
            }

            try
            {
                var json = File.ReadAllText(_databaseFilePath);

                // Десериализация в словарь
                var products = JsonSerializer.Deserialize<Dictionary<Guid, Product>>(json) ?? new Dictionary<Guid, Product>();

                _logger.LogInformation("Загружено {Count} продуктов из файла.", products.Count);
                return products;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка при десериализации файла базы данных. Создан пустой словарь продуктов.");
                return new Dictionary<Guid, Product>();
            }
        }

        /// <summary>
        /// Сохраняет текущее состояние словаря продуктов в файл.
        /// </summary>
        private void WriteToFile()
        {
            bool hasMutex = false;
            try
            {
                // Пытаемся захватить мьютекс в течение 5 секунд
                hasMutex = _fileMutex.WaitOne(TimeSpan.FromSeconds(5));
                if (hasMutex)
                {
                    var json = JsonSerializer.Serialize(_products, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(_databaseFilePath, json); // Синхронная запись
                    _logger.LogInformation("Данные успешно записаны в файл.");
                }
                else
                {
                    _logger.LogError("Не удалось получить мьютекс для записи в файл.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в файл.");
            }
            finally
            {
                // Освобождаем мьютекс только если он был успешно захвачен
                if (hasMutex)
                {
                    _fileMutex.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// Добавляет новый продукт.
        /// </summary>
        /// <param name="product">Данные продукта.</param>
        /// <returns>Добавленный продукт.</returns>
        public async Task<Product> Add(Product product)
        {
            product.Id = Guid.NewGuid();
            _products[product.Id] = product; // Добавляем в оперативную память
            await Task.Run(() => WriteToFile()); // Сохраняем изменения на диск
            return product;
        }

        /// <summary>
        /// Удаляет продукт по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор продукта.</param>
        /// <returns>Удаленный продукт или null, если продукт не найден.</returns>
        public async Task<Product?> Remove(Guid id)
        {
            if (_products.TryGetValue(id, out var product))
            {
                _products.Remove(id); // Удаляем из оперативной памяти
                await Task.Run(() => WriteToFile()); // Сохраняем изменения на диск
            }
            return product;
        }

        /// <summary>
        /// Редактирует существующий продукт.
        /// </summary>
        /// <param name="id">Идентификатор продукта.</param>
        /// <param name="product">Новые данные продукта.</param>
        /// <returns>Отредактированный продукт или null, если продукт не найден.</returns>
        public async Task<Product?> Edit(Guid id, Product product)
        {
            if (_products.TryGetValue(id, out var existingProduct))
            {
                existingProduct.Name = product.Name;
                existingProduct.Definition = product.Definition;
                existingProduct.Price = product.Price;
                existingProduct.Image = product.Image;

                await Task.Run(() => WriteToFile()); // Сохраняем изменения на диск
            }
            return existingProduct;

        }

        /// <summary>
        /// Ищет продукт по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор продукта.</param>
        /// <returns>Найденный продукт или null, если продукт не найден.</returns>
        public Task<Product?> Search(Guid id)
        {
            _products.TryGetValue(id, out var product);
            return Task.FromResult(product);
        }

        public void PrintProducts()
        {
            Console.WriteLine("Текущее содержимое _products:");
            foreach (var (id, product) in _products)
            {
                Console.WriteLine($"ID: {id}, Название: {product.Name}, Цена: {product.Price}");
            }
        }

    }
}