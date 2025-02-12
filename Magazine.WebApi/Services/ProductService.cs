using System;
using System.Collections.Generic;
using System.Linq;
using Magazine.Core.Models;
using Magazine.Core.Services;

namespace Magazine.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products = new(); // Временное хранилище

        public Product Add(Product product)
        {
            product.Id = Guid.NewGuid();
            _products.Add(product);
            return product;
        }

        public Product Remove(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
            return product;
        }

        public Product Edit(Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Definition = product.Definition;
                existingProduct.Price = product.Price;
                existingProduct.Image = product.Image;
            }
            return existingProduct;
        }

        public Product Search(Guid id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
    }
}
