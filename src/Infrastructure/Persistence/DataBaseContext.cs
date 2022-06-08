using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence
{
    public class DataBaseContext : DbContext
    {
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Outbox> Outbox { get; set; }
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }
        public DataBaseContext() { }
    }
}
