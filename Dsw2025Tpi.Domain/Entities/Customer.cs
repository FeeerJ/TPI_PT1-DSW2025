using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Customer : EntityBase
    {
        public string? email {  get; set; }
        public string? phoneNumber { get; set; }
        public string? name { get; set; }

        /*Un cliente puede realizar muchas ordenes*/
        public virtual ICollection<Order>? orders { get; set; }

        public Customer(string email, string phoneNumber, string name)
        {
            this.email = email;
            this.phoneNumber = phoneNumber;
            this.name = name;
            Id = Guid.NewGuid();
            orders = new HashSet<Order>();
        }


    }
}
