using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Orders.Models
{
    public class Product : Entity<string>
    {
        public int Quantity { get; set; }

        public Product(string id, int quantity) : base(id)
        {
            Quantity = quantity;
        }
    }
}
