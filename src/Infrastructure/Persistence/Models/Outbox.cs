using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infrastructure.Persistence.Models
{
    public class Outbox
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }

        [Column(TypeName = "jsonb")]
        public string Data { get; set; }
        public DateTime Created { get; set; }
    }
}
