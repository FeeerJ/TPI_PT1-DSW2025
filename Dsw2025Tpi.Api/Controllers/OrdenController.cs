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

        [HttpPost()]
        [Authorize(Roles = "Admin, User")] /*Podria hacerlo el usuario, con la condicion de que sea solo sus ordenes*/
        public async Task<IActionResult> AddOrder([FromBody] OrderModel.OrderRequest request)
        {
                var orden = await _service.AddOrder(request);
                return Created($"/api/orders/{orden.Id}", orden);

        }
        [HttpGet]
        [Authorize(Roles = "Admin, User")] /*Podria hacerlo el usuario, con la condicion de que sea solo sus ordenes*/
        public async Task<IActionResult> GetOrders([FromQuery] OrderModel.OrderFilter filter)
        {
                var orders = await _service.GetOrders(filter);
                return Ok(orders);

        }


    
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderId(Guid id)
        {
          
                var order = await _service.GetOrderById(id);
                if (order == null) return NotFound();
                return Ok(order);

        }
    
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, int codigo)
        {
                var order = await _service.UpdateOrderStatus(id, codigo);
                if (order == null) return NotFound();
                return Ok(order);
         
        }
    }
}
