using Microsoft.AspNetCore.Mvc;
using Magazine.Core.Models;
using Magazine.Core.Services;
using System;

namespace Magazine.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("add")]
        public ActionResult<Product> Add([FromBody] Product product)
        {
            return Ok(_productService.Add(product));
        }

        [HttpDelete("remove/{id}")]
        public ActionResult<Product> Remove(Guid id)
        {
            var removedProduct = _productService.Remove(id);
            if (removedProduct == null) return NotFound();
            return Ok(removedProduct);
        }

        [HttpPut("edit")]
        public ActionResult<Product> Edit([FromBody] Product product)
        {
            var updatedProduct = _productService.Edit(product);
            if (updatedProduct == null) return NotFound();
            return Ok(updatedProduct);
        }

        [HttpGet("search/{id}")]
        public ActionResult<Product> Search(Guid id)
        {
            var product = _productService.Search(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}
