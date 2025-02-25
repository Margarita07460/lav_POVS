using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Magazine.Core.Models;
using Magazine.Core.Services;

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

        // Добавление продукта
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
            return Ok(result);
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
            return Ok(result);
        }
    }
}