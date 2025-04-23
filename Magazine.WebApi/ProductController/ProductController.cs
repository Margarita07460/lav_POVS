using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Magazine.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        // Внедрение зависимости через конструктор
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        //Добавление продукта
       [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            var result = await _productService.Add(product);
            return Ok(result);
        }


        // Удаление продукта
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await _productService.Remove(id);
            if (result == null)
            {
                return NotFound(); // Продукт не найден
                //return -1;
            }
            return Ok(result); // Продукт успешно удален
        }

        // Редактирование продукта
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, Product product)
        {
            var result = await _productService.Edit(id, product);
            return Ok(result);
        }

        // Поиск продукта
        [HttpGet("{id}")]
        public async Task<IActionResult> Search(Guid id)
        {
            var result = await _productService.Search(id);
            if (result == null)
            {
                return NotFound(); // Продукт не найден
            }
            return Ok(result); // Продукт найден
        }
    }
}

