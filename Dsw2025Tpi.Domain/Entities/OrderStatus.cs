using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public enum OrderStatus
    {
        PENDING = 0,
        PROCESSING = 1,
        SHIPPED = 2,
        DELIVERED = 3,
        CANCELLED = 4
    }
}
