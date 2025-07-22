using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }
        public decimal subTotal {  get; set; }

        public Guid orderId { get; set; }
        public virtual Order order { get; set; }
        
        public Guid productId { get; set; }
        public virtual Product product { get; set; }

        public OrderItem() { }
        public OrderItem(int quantity, decimal unitPrice, decimal subTotal, Guid orderId, Guid productId)
        {
            this.quantity = quantity;
            this.unitPrice = unitPrice;
            this.subTotal = subTotal;
            this.orderId = orderId;
            this.productId = productId;
            Id = Guid.NewGuid();
        }


    }
}
