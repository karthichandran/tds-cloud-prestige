using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.Customers.Commands.UpdateCustomer;
using ReProServices.Domain.Entities;

namespace ReProServices.Application.Customers.Commands
{
    public class TracesPasswordSentDateCommand : IRequest<bool>
    {
        public int CustomerId { get; set; }

        public class TracesPasswordSentDateCommandHandler : IRequestHandler<TracesPasswordSentDateCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public TracesPasswordSentDateCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<bool> Handle(TracesPasswordSentDateCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = _context.Customer.FirstOrDefault(x => x.CustomerID == request.CustomerId);
                    if (entity != null)
                    {
                        
                        entity.TracesPwdSentDate = DateTime.UtcNow;
                        _context.Customer.Update(entity);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("PAN already exists");
                }
            }
        }
    }
}
