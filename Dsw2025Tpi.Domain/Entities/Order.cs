using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public DateTime date {  get; set; }
        public string? shippingAddress{ get; set; }
        public string? billingAddress { get; set; }
        public string? notes { get; set; }
        public decimal totalAmount { get; set; }
        public OrderStatus status { get; set; } = OrderStatus.PENDING;

        public Guid customerId { get; set; }
        public virtual Customer? customer { get; set; }

        public virtual ICollection<OrderItem>? orderItems { get; set; }

        public Order(DateTime date, string shippingAddress, string billingAddress, string notes, decimal totalAmount)
        {
            this.date = date;
            this.shippingAddress = shippingAddress;
            this.billingAddress = billingAddress;
            this.notes = notes;
            Id = Guid.NewGuid();
            this.totalAmount = totalAmount;
     
           

        }


    }
}
