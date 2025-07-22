using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Product : EntityBase
    {

        public Product(string sku, string name , decimal currentUnitPrice, string internalCode, string description , int stockQuantity, bool isActive)
        {
            Sku = sku;
            Name = name;
            CurrentUnitPrice = currentUnitPrice;
            InternalCode = internalCode;
            Description = description;
            StockQuantity = stockQuantity;
            IsActive = isActive;
            Id = Guid.NewGuid();
            OrderItems = new HashSet<OrderItem>();

        }
        public string? Sku {  get; set; }
        public string? Name { get; set; }
        public decimal CurrentUnitPrice { get; set; }
        public string? InternalCode { get; set; }
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }

    }
}
