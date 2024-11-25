using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Domain.Entities.ClientPortal;

namespace ReProServices.Infrastructure.Persistence
{
    public class ClientPortalDbContext : DbContext,IClientPortalDbContext
    {
        public ClientPortalDbContext(DbContextOptions<ClientPortalDbContext> options) : base(options) { }
        public DbSet<LoginUser> LoginUser { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
