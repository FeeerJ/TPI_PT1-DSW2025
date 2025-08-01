using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Dsw2025Tpi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductManagerService _service;

        public ProductController(ProductManagerService service)
        {
            _service = service;
        }

        [HttpPost()] 
        public async Task<IActionResult> AddProduct([FromBody] ProductModel.ProductRequest request)
        {
            var producto = await _service.AddProduct(request);
            return Created($"/api/productos/{producto.id}", producto);
        }



        [HttpGet] 
        public async Task<IActionResult> GetProducts()
        {
            var products = await _service.GetProducts();
            if (products == null || !products.Any()) return NoContent();

            return Ok(products);
        }


        [HttpGet("{id}")] 
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _service.GetProductID(id);
            if (product == null) return NotFound();
            return Ok(product);


        }

        [HttpPut("{id}")] 
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.ProductRequest request)
        {
          
              var product = await _service.UpdateProduct(request, id);
               return Ok(product);
        }

        [HttpPatch("{id}")] 
        public async Task<IActionResult> DisableProduct(Guid id)
        {
                var state = await _service.DisableProduct(id);
                return NotFound("El producto no existe o ya esta deshabilitado");
                
         
        }

    }
}
