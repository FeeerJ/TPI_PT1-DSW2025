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

        /*
        public async Task<IEnumerable<OrderModel.OrderResponse>?> GetOrders()
        {
            var orderList = await _repository.GetAll<Order>(include: "orderItems");
            if (orderList == null || !orderList.Any()) return new List<OrderModel.OrderResponse>();
            
            var orders = orderList.Select(order =>
            {
                return new OrderModel.OrderResponse(
                    Id: order.Id,
                    customerId: order.customerId,
                    shippingAddress: order.shippingAddress,
                    billingAddress: order.billingAddress,
                    notes: order.notes,
                    date: order.date,
                    totalAmount: order.totalAmount,
                  
                    OrderItems: order.orderItems?
                                    .Select(item => new OrderModel.OrderItemResponse(
                                        productId: item.productId,
                                        unitPrice: item.unitPrice,    
                                        quantity: item.quantity,     
                                        subTotal: item.subTotal     
                                    )).ToList() ?? new List<OrderModel.OrderItemResponse>(), 
                    status: order.status.ToString()
                );
            }).ToList(); 

            return orders;
        }*/
        public async Task<IEnumerable<OrderModel.OrderResponse>?> GetOrders(OrderModel.OrderFilter filter)
        {
            var orderList = await _repository.GetAll<Order>(include: "orderItems");
            if (orderList == null || !orderList.Any()) return new List<OrderModel.OrderResponse>();

            /**/
            if(!string.IsNullOrWhiteSpace(filter.status) && Enum.TryParse<OrderStatus>(filter.status, true, out var parsedStatus))
                orderList = orderList.Where(o => o.status == parsedStatus).ToList();
            /**/
            if(filter.customerId.HasValue)
                orderList = orderList.Where(o => o.customerId == filter.customerId.Value).ToList();
            /**/
            var paginationOrder = orderList
                .Skip((filter.pageNumer - 1) * filter.pageSize)
                .Take(filter.pageSize)
                .ToList();


            var orders = orderList.Select(order =>
            {
                return new OrderModel.OrderResponse(
                    Id: order.Id,
                    customerId: order.customerId,
                    shippingAddress: order.shippingAddress,
                    billingAddress: order.billingAddress,
                    notes: order.notes,
                    date: order.date,
                    totalAmount: order.totalAmount,

                    OrderItems: order.orderItems?
                                    .Select(item => new OrderModel.OrderItemResponse(
                                        productId: item.productId,
                                        unitPrice: item.unitPrice,
                                        quantity: item.quantity,
                                        subTotal: item.subTotal
                                    )).ToList() ?? new List<OrderModel.OrderItemResponse>(),
                    status: order.status.ToString()
                );
            }).ToList();

            return orders;
        }
        public async Task<OrderModel.OrderResponse> AddOrder(OrderModel.OrderRequest request)
        {
            if (
                 request == null || request.CustomerId == Guid.Empty ||
                 string.IsNullOrWhiteSpace(request.shippingAddress)  ||
                 string.IsNullOrWhiteSpace(request.billingAddress)   ||
                 request.orderItems == null || !request.orderItems.Any()
               ) throw new ArgumentException("Argumentos invalidos.");
            

            var customer = await _repository.GetById<Customer>(request.CustomerId);
            if (customer == null) throw new NotFoundException($"Cliente con ID {request.CustomerId} no encontrado.");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;
            foreach (var itemRequest in request.orderItems)
            {
                var product = await _repository.GetById<Product>(itemRequest.ProductId);
                if (product == null || !product.IsActive) throw new NotFoundException($"Producto con ID {product.Id} no encontrado.");
                if (product.StockQuantity < itemRequest.quantity) throw new ArgumentException($"Stock insuficiente para el producto {product.Name}.");
                
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

            var responseItems = order.orderItems.Select(oi => new OrderModel.OrderItemResponse( oi.productId, oi.unitPrice,oi.quantity,oi.subTotal)).ToList();

            return new OrderModel.OrderResponse(order.Id,order.customerId,order.shippingAddress,order.billingAddress, order.notes,order.date,order.totalAmount,responseItems,order.status.ToString());

        }

        public async Task<OrderModel.OrderResponse?> GetOrderById(Guid id)
        {
            // Cargar la Orden e incluir explícitamente sus OrderItems.
            // "Items" debe ser el nombre de la propiedad de navegación ICollection<OrderItem> en tu entidad Order.
            var order = await _repository.First<Order>(o => o.Id == id, include: "orderItems");

            if (order == null) throw new NotFoundException($"Orden con ID {id} no encontrada.");
    

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

        public async Task<OrderModel.OrderStatusResponse?> UpdateOrderStatus(Guid id, int codigoEstado)
        {
            // 1. Validar el código de estado: Asegurarse de que es un valor válido para el enum OrderStatus.
            if (!Enum.IsDefined(typeof(OrderStatus), codigoEstado)) throw new ArgumentException($"El código de estado '{codigoEstado}' no es un valor válido para OrderStatus.");
            var nuevoEstado = (OrderStatus)codigoEstado;
            var order = await _repository.First<Order>(o => o.Id == id, include: "orderItems");

            if (order == null) throw new NotFoundException($"Orden con ID {id} no encontrada.");
            order.status = nuevoEstado;

            await _repository.Update(order);
             var orderResponse = new OrderModel.OrderStatusResponse( newStatus: order.status.ToString());

            return orderResponse;
        }

    }
}
