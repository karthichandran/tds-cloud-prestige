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
    public class UpdateTracesPasswordCommand:IRequest<bool>
    {
        public CustomerDto CustomerDto { get; set; }

        public class UpdateTracesPasswordCommandHandler : IRequestHandler<UpdateTracesPasswordCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public UpdateTracesPasswordCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<bool> Handle(UpdateTracesPasswordCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = _context.Customer
                        .FirstOrDefault(x => x.CustomerID == request.CustomerDto.CustomerID);
                    if (entity != null)
                    {
                        entity.TracesPassword = request.CustomerDto.TracesPassword;
                        entity.IsTracesRegistered = true;
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
