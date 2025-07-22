using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record ProductModel
    {
        public record ProductRequest(string sku, string name, decimal currentUnitPrice, string internalCode, string description, int stockQuantity, bool isActive);
        public record ProductResponse (Guid id, string sku, string name, decimal currentUnitPrice, string internalCode, string description, int stockQuantity, bool isActive);
        public record ProductResponseUpdate(Guid id, string Sku, string Name, decimal CurrentUnitPrice, string InternalCode, string Description, int StockQuantity, bool IsActive);

        public record ProductResponseID(Guid id);
        


    }
}
