using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magazine.Core.Models;


namespace Magazine.Core.Services
{
    public interface IProductService
    {
        Product Add(Product product); // добавление нового продукта
        Product Remove(Guid id); // удаление продукта
        Product Edit(Product product); // редактирование продукта
        Product Search(Guid id); // поиск продукта по Id
    }
}
