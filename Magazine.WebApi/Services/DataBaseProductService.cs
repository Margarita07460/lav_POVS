using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Magazine.WebApi.Services
{
    public class DataBaseProductService : IProductService
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<DataBaseProductService> _logger;

        public DataBaseProductService(
            ApplicationContext context,
            ILogger<DataBaseProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Product> Add(Product product)
        {
            try
            {
                product.Id = Guid.NewGuid();
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product added: {product.Id}");
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                throw;
            }
        }

        //public async Task<Product> Add(Product product)
        //{
        //    // Удаляем проверку на существующий ID
        //    // Просто добавляем продукт
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();
        //    return product;
        //}

        public async Task<Product?> Remove(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product removed: {id}");
            }
            else
            {
                _logger.LogWarning($"Product not found for removal: {id}");
            }
            return product;
        }

        public async Task<Product?> Edit(Guid id, Product product)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning($"Product not found for editing: {id}");
                    return null;
                }

                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product updated: {id}");
                return existingProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error editing product: {id}");
                throw;
            }
        }

        public async Task<Product?> Search(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product not found: {id}");
            }
            return product;
        }
    }
}