using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.WebApi.Controllers.Orders
{
    public class Request
    {
        public Guid Id { get; set; }

        public Dictionary<string, int> Products { get; set; }
    }
}
