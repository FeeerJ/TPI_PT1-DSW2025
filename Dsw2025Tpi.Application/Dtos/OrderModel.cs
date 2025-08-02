using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record OrderModel
    {
        public record OrderItemModel(
            [property: JsonPropertyName("productoId")] Guid ProductId,int quantity
        ); 

        public record OrderItemResponse( Guid productId,decimal unitPrice, int quantity,decimal subTotal);


        public record OrderRequest(
            [property: JsonPropertyName("customerId")] Guid CustomerId,string shippingAddress,string billingAddress,string notes,List<OrderItemModel> orderItems
            );

        public record OrderFilter(string? status =null, Guid? customerId=null, int pageNumer=1, int pageSize=10);
        public record OrderStatusResponse(string newStatus);
        public record OrderResponse(Guid Id,Guid customerId,string shippingAddress,string billingAddress, string notes,DateTime date,
            decimal totalAmount,List<OrderItemResponse> OrderItems,string status);

    }
}
