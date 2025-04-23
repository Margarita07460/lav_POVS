//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel.DataAnnotations;       // Для [Key]
//using System.ComponentModel.DataAnnotations.Schema; // Для [Index], [DatabaseGenerated]


//namespace Magazine.Core.Models
//{
//    public class Product
//    {
//        public Guid Id { get; set; }
//        public string Name { get; set; } = string.Empty;
//        public string Definition { get; set; } = string.Empty;
//        public decimal Price { get; set; }
//        public string Image { get; set; } = string.Empty;
//    }
//}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Magazine.Core.Models
{
    [Index(nameof(Id), Name = "IX_Products_Id", IsUnique = true)]
    public class Product
    {
      
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("Name", TypeName = "TEXT")]
        public string Name { get; set; } = string.Empty;

        [Column("Definition", TypeName = "TEXT")]
        public string Definition { get; set; } = string.Empty;

        [Required]
        [Column("Price", TypeName = "DECIMAL(10,2)")]
        public decimal Price { get; set; }

        [Column("Image", TypeName = "TEXT")]
        public string Image { get; set; } = string.Empty;

        [Column("Weight", TypeName = "DECIMAL(5,2)")]
        public decimal Weight { get; set; } = 0.1m;

        // Новое поле - состояние товара
        [Column("Status", TypeName = "TEXT")]
        public string Status { get; set; } = "Available"; // Значение по умолчанию
    }
}