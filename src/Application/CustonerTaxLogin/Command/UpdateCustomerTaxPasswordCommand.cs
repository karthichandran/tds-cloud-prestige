using MediatR;
using ReProServices.Application.Common.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using ReProServices.Domain.Entities;
using System.Linq;
using System;
using System.Collections.Generic;

namespace ReProServices.Application.CustonerTaxLogin.Command
{
    public class UpdateCustomerTaxPasswordCommand : IRequest<bool>
    {
        public List<CustomerTaxPasswordDto> customers { get; set; }

        public class UpdateCustomerTaxPasswordCommandHandler : IRequestHandler<UpdateCustomerTaxPasswordCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public UpdateCustomerTaxPasswordCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<bool> Handle(UpdateCustomerTaxPasswordCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    foreach (var cus in request.customers)
                    {
                        if (!cus.IsSelected)
                            continue;

                        var cp = _context.CustomerTaxLogin.Where(x => x.CustomerTaxLoginId == cus.CustomerTaxLoginId).FirstOrDefault();

                        if (cp == null)
                            continue;

                        var entity = _context.Customer.Where(c => c.CustomerID == cp.CustomerId).FirstOrDefault();

                        if (cp.IsOptOut.Value)
                        {
                            //entity.InvalidPanDate = cp.AsOfDate;
                            entity.CustomerOptingOutDate = cp.AsOfDate;
                            entity.CustomerOptedOut = true;
                        }
                        else
                            entity.IncomeTaxPassword = cp.TaxPassword;

                        cp.IsProcessed = true;
                        _context.Customer.Update(entity);
                        await _context.SaveChangesAsync(cancellationToken);

                        _context.CustomerTaxLogin.Update(cp);
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
