using Magazine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Core.Services
{
    public interface IProductService
    {
        Task<Product> Add(Product product);
        Task<Product?> Remove(Guid id);
        Task<Product?> Edit(Product product);
        Task<Product?> Search(Guid id);
    }
}