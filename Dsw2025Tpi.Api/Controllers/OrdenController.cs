using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

        /*ENDPOINT 06 CREAR UNA NUEVA ORDEN*/
     
        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel.OrderRequest request)
        {
                var orden = await _service.AddOrder(request);
                return Created($"/api/orders/{orden.Id}", orden);

        }

        /*ENDPOINT 07 OBTENER TODAS LAS ORDENES*/
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrders([FromQuery] OrderModel.OrderFilter filter)
        {
                var orders = await _service.GetOrders(filter);
                return Ok(orders);

        }


        [AllowAnonymous]
        [HttpGet("{id}")] /*EndPoint 08 : Busca obtener un producto por ID */
        public async Task<IActionResult> GetOrderId(Guid id)
        {
          
                var order = await _service.GetOrderById(id);
                if (order == null) return NotFound();
                return Ok(order);

        }
        [AllowAnonymous]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, int codigo)
        {
                var order = await _service.UpdateOrderStatus(id, codigo);
                if (order == null) return NotFound();
                return Ok(order);
         
        }
    }
}
