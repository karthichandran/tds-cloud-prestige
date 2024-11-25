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
   public class UpdateMailStatusCommand : IRequest<bool>
    {
        public Guid OwnershipId { get; set; }
        public int CustomerID { get; set; }
        public bool IsOwner { get; set; }
        public DateTime date { get; set; }


        public class UpdateMailStatusCommandHandler : IRequestHandler<UpdateMailStatusCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public UpdateMailStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<bool> Handle(UpdateMailStatusCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = _context.CustomerProperty.FirstOrDefault(x => x.CustomerId == request.CustomerID && x.OwnershipID== request.OwnershipId);
                    if (entity != null)
                    {
                        if(request.IsOwner)
                        entity.ITpwdMailStatus= request.date;
                        else
                        entity.CoOwnerITpwdMailStatus= request.date;

                        _context.CustomerProperty.Update(entity);
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
