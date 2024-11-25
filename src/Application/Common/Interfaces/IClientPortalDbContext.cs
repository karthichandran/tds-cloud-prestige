using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReProServices.Domain.Entities.ClientPortal;

namespace ReProServices.Application.Common.Interfaces
{
    public interface IClientPortalDbContext
    {
        DbSet<LoginUser> LoginUser { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
