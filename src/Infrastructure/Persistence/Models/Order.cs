using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infrastructure.Persistence.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        [Column(TypeName = "jsonb")]
        public string Data { get; set; }
    }
}
