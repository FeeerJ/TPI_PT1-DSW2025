using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dsw2025Tpi.Application.Dtos.OrderModel;

namespace Dsw2025Tpi.Application.Services
{
    public class OrderManagerService
    {
        private readonly IRepository _repository;

        public OrderManagerService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<OrderModel.OrderResponse>?> GetOrders()
        {
            // 1. Obtener todas las órdenes del repositorio, incluyendo sus ítems.
            // Asegúrate que "Items" es el nombre de la propiedad de navegación ICollection<OrderItem> en tu entidad Order.
            var listaOrdenes = await _repository.GetAll<Order>(include: "orderItems");

            // 2. Manejo de caso sin órdenes: Devolver una lista vacía de DTOs, no lanzar excepción.
            // Esto es estándar RESTful para GET de colecciones sin resultados.
            if (listaOrdenes == null || !listaOrdenes.Any())
            {
                return new List<OrderModel.OrderResponse>(); // Devuelve una lista vacía de DTOs
            }

            // 3. Mapear cada entidad Order a su DTO OrderModel.OrderResponse
            var listaRespuesta = listaOrdenes.Select(order =>
            {
                return new OrderModel.OrderResponse(
                    Id: order.Id,
                    customerId: order.customerId, // Asumo que en la entidad es Order.CustomerId (PascalCase)
                    shippingAddress: order.shippingAddress,
                    billingAddress: order.billingAddress,
                    notes: order.notes,
                    date: order.date,
                    totalAmount: order.totalAmount,
                    // 4. Mapear la colección anidada de OrderItems a OrderModel.OrderItemResponse
                    OrderItems: order.orderItems? // Propiedad de navegación de la entidad Order
                                    .Select(item => new OrderModel.OrderItemResponse(
                                        productId: item.productId,
                                        unitPrice: item.unitPrice,    // Propiedad de la entidad OrderItem
                                        quantity: item.quantity,      // Propiedad de la entidad OrderItem
                                        subTotal: item.subTotal       // Propiedad de la entidad OrderItem
                                    ))
                                    .ToList() // Convertir a List<OrderModel.OrderItemResponse>
                                    ?? new List<OrderModel.OrderItemResponse>(), // Si Items es null, devolver lista vacía
                    status: order.status.ToString() // Convertir el enum a string para el DTO
                );
            }).ToList(); // Convertir el resultado del Select a una List<OrderModel.OrderResponse>

            return listaRespuesta;
        }


        
        public async Task<OrderModel.OrderResponse> AgregarOrden(OrderModel.OrderRequest request)
        {
            if (request == null ||
                request.CustomerId == Guid.Empty ||
                string.IsNullOrWhiteSpace(request.shippingAddress) ||
                string.IsNullOrWhiteSpace(request.billingAddress) ||
                request.orderItems == null ||
                !request.orderItems.Any())
            {
                throw new ArgumentException("Argumentos invalidos o incompletos");
            }
            var customer = await _repository.GetById<Customer>(request.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException($"Cliente con ID {request.CustomerId} no encontrado");


            }
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;
            foreach (var itemRequest in request.orderItems)
            {
                var product = await _repository.GetById<Product>(itemRequest.ProductId);
                if (product == null || !product.IsActive)
                {
                    throw new ArgumentException($"Producto con ID {product.Id} does not exist");
                }
                if (product.StockQuantity < itemRequest.quantity)
                {
                    throw new ArgumentException($"Stock insuficiente para el producto {product.Name}");
                }
                var subTotal = itemRequest.quantity * product.CurrentUnitPrice;
                totalAmount += subTotal;

                var orderItem = new OrderItem
                {
                    quantity = itemRequest.quantity,
                    unitPrice = product.CurrentUnitPrice,
                    subTotal = subTotal,
                    productId = product.Id
                };
                orderItems.Add(orderItem);

                product.StockQuantity -= itemRequest.quantity;
                await _repository.Update(product);
            }

            var order = new Order(DateTime.Now, request.shippingAddress, request.billingAddress, request.notes, totalAmount);
            order.customerId = customer.Id;
            order.orderItems = orderItems;
            await _repository.Add(order);

            var responseItems = order.orderItems.Select(oi => new OrderModel.OrderItemResponse(
               oi.productId,
               oi.unitPrice,
               oi.quantity,
               oi.subTotal

             )).ToList();

            return new OrderModel.OrderResponse(
               order.Id,
               order.customerId,
               order.shippingAddress,
               order.billingAddress,
               order.notes,
               order.date,
               order.totalAmount,
               responseItems,
               order.status.ToString()
           );

        }


        public async Task<OrderModel.OrderResponse?> GetOrderById(Guid id)
        {
            // Cargar la Orden e incluir explícitamente sus OrderItems.
            // "Items" debe ser el nombre de la propiedad de navegación ICollection<OrderItem> en tu entidad Order.
            var order = await _repository.First<Order>(o => o.Id == id, include: "orderItems");

            if (order == null)
            {
                // Si la orden no se encuentra, lanzar una excepción específica
                // Esto permite que el controlador (API) devuelva un 404 Not Found.
                throw new ArgumentException($"Orden con ID {id} no encontrada.");
            }

            // Mapear la entidad 'Order' a tu DTO 'OrderModel.OrderResponse'
            var orderResponse = new OrderModel.OrderResponse(
                Id: order.Id,
                customerId: order.customerId, // Propiedad CustomerId de la entidad Order
                shippingAddress: order.shippingAddress,
                billingAddress: order.billingAddress,
                notes: order.notes,
                date: order.date,
                totalAmount: order.totalAmount,
                // Mapear la colección de OrderItem (entidades) a List<OrderModel.OrderItemResponse> (DTOs)
                OrderItems: order.orderItems? // Propiedad de navegación de la entidad Order
                                    .Select(item => new OrderModel.OrderItemResponse(
                                        productId: item.productId,    // Propiedad de la entidad OrderItem
                                        unitPrice: item.unitPrice,    // Propiedad de la entidad OrderItem
                                        quantity: item.quantity,      // Propiedad de la entidad OrderItem
                                        subTotal: item.subTotal       // Propiedad de la entidad OrderItem
                                                                      // Nota: Tu OrderModel.OrderItemResponse NO incluye 'name' o 'description'
                                                                      // Asegúrate de que esto es intencional, ya que estaban en OrderItemModel (input)
                                    ))
                                    .ToList() // Convertir el resultado del Select a una List<OrderItemResponse>
                                    ?? new List<OrderModel.OrderItemResponse>(), // Si order.Items es null, devolver una lista vacía
                status: order.status.ToString() // Convertir el enum OrderStatus a su representación de string para el DTO
            );

            return orderResponse;
        }

        public async Task<OrderModel.OrderResponse?> ModificarEstado(Guid id, int codigoEstado)
        {
            // 1. Validar el código de estado: Asegurarse de que es un valor válido para el enum OrderStatus.
            if (!Enum.IsDefined(typeof(OrderStatus), codigoEstado))
            {
                throw new ArgumentException($"El código de estado '{codigoEstado}' no es un valor válido para OrderStatus.");
            }

            // Convertir el código numérico al tipo enum OrderStatus.
            var nuevoEstado = (OrderStatus)codigoEstado;

            // 2. Buscar la orden existente en el repositorio.
            var order = await _repository.First<Order>(o => o.Id == id, include: "orderItems");

            if (order == null)
            {
                // Si la orden no se encuentra, lanzar NotFoundException.
                throw new ArgumentException($"Orden con ID {id} no encontrada para actualizar su estado.");
            }

            // 3. Actualizar el estado de la orden.
            // La especificación dice "solo modificar el estado", así que esto es lo único que cambiamos.
            order.status = nuevoEstado;

            // 4. Guardar los cambios a través del repositorio.
            // Tu _repository.Modificar() debería manejar la persistencia de esta entidad actualizada.
            await _repository.Update(order);

            // 5. Devolver un DTO de respuesta para la orden actualizada.
            // Puedes devolver el DTO completo de la orden actualizada.
            var orderResponse = new OrderModel.OrderResponse(
                Id: order.Id,
                customerId: order.customerId,
                shippingAddress: order.shippingAddress,
                billingAddress: order.billingAddress,
                notes: order.notes,
                date: order.date,
                totalAmount: order.totalAmount,
                // Mapear los OrderItems si los quieres en la respuesta, similar a GetOrderById
                OrderItems: order.orderItems?
                                .Select(item => new OrderModel.OrderItemResponse(
                                    productId: item.productId,
                                    unitPrice: item.unitPrice,
                                    quantity: item.quantity,
                                    subTotal: item.subTotal
                                    //productName: item.product?.Name ?? "N/A", // Si Product se carga
                                    // productDescription: item.product?.Description ?? "N/A" // Si Product se carga
                                ))
                                .ToList() ?? new List<OrderModel.OrderItemResponse>(),
                status: order.status.ToString() // Devuelve el estado como string
            );

            return orderResponse;
        }

    }
}
