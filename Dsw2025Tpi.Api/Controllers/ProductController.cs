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

        [HttpPost()] /*Endpoint 01 : Busca permitir crear un producto*/
        public async Task<IActionResult> AgregarProducto([FromBody] ProductModel.ProductRequest request)
        {
            try
            {
                var producto = await _service.AgregarProducto(request);
                return Ok(producto);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DuplicatedEntityException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                return Problem("Se produjo un error al guardar el producto");
            }
        }

        [HttpGet] /*EndPoint 02 : Busca obtener todos los productos */
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _service.GetProducts();
            if (products == null || !products.Any()) return NoContent();

            return Ok(products);
        }


        [HttpGet("{id}")] /*EndPoint 03 : Busca obtener un producto por ID */
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _service.GetProductID(id);
            if (product == null) return NoContent();
            return Ok(product);


        }

        [HttpPut] /*EndPoint 04 : Busca actualizar un producto por ID */
        public async Task<IActionResult> ModificarProducto([FromRoute]Guid id, [FromBody] ProductModel.ProductRequest request)
        {
            await _service.ModificarProducto(request, id);
            return NoContent();



        }

        [HttpPatch] /*EndPoint 05: Inhabilitar un producto por ID*/
        public async Task<IActionResult> InhabilitarProducto(Guid id)
        {
            await _service.DeshabilitarProducto(id);
            return NoContent();

        }


    }
 }
