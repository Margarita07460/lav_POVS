using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Magazine.Core.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Уникальный идентификатор
        public string Name { get; set; }  // Название товара
        public string Definition { get; set; } // Описание товара
        public decimal Price { get; set; } // Стоимость товара
        public string Image { get; set; } // Изображение товара (путь к файлу)

        public Product() { }

        public Product(string name, string definition, decimal price, string image)
        {
            Name = name;
            Definition = definition;
            Price = price;
            Image = image;
        }
    }
}
