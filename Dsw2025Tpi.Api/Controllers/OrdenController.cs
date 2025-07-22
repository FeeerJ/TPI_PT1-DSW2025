using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Dsw2025Tpi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/orders")]
    public class OrdenController : ControllerBase
    {
        private readonly OrderManagerService _service;

        public OrdenController(OrderManagerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _service.GetOrders();
            if (orders == null || !orders.Any()) return NoContent();

            return Ok(orders);
        }

        [HttpPost()]
        public async Task<IActionResult> AgregarOrden([FromBody] OrderModel.OrderRequest request)
        {
            try
            {
                var orden = await _service.AgregarOrden(request);
                return Ok(orden);
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
                return Problem("Se produjo un error al guardar la orden");
            }

        }

        [HttpGet("{id}")] /*EndPoint 03 : Busca obtener un producto por ID */
        public async Task<IActionResult> GetOrderId(Guid id)
        {
            var order = await _service.GetOrderById(id);
            if (order == null) return NoContent();
            return Ok(order);

        }

        [HttpPut]
        public async Task<IActionResult> ModificarEstado(Guid id, int codigo)
        {
            var order = await _service.ModificarEstado(id, codigo);
            if (order == null) return NoContent();
            return Ok(order);
        }
    }
}
