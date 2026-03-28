using Microsoft.EntityFrameworkCore;
using Ordering_API.Common;
using Ordering_API.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering_API.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = "Bolt";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        entry.Entity.LastModifiedBy = "Bolt";
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
