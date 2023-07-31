using MediatR;
using ReProServices.Application.Common.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using ReProServices.Domain.Entities;
using System.Linq;
using System;

namespace ReProServices.Application.CustonerTaxLogin.Command
{
    public class CreateCustomerTaxLoginCommand : IRequest<bool>
    {
        public CustomerTaxLoginDetails customers { get; set; }

        public class CreateCustomerTaxLoginCommandHandler : IRequestHandler<CreateCustomerTaxLoginCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public CreateCustomerTaxLoginCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<bool> Handle(CreateCustomerTaxLoginCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var custVM = _context.CustomerTaxLogin.Where(x => x.UnitNo == request.customers.UnitNo).ToList();
                    var isOptOut = request.customers.IsOptedOut;
                    var unitNo = request.customers.UnitNo;
                    if (custVM.Count() == 0)
                    {
                        foreach (var cus in request.customers.customers)
                        {

                            var entity = new CustomerTaxLogin
                            {
                                CustomerId = cus.CustomerID,
                                UnitNo = unitNo,
                                TaxPassword = cus.IncomeTaxPassword,
                                IsOptOut = isOptOut,
                                IsProcessed = false,
                                AsOfDate = request.customers.AsOfDate
                            };
                            await _context.CustomerTaxLogin.AddAsync(entity, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);
                        }

                    }
                    else
                    {
                        foreach (var cus in request.customers.customers)
                        {
                            var entity = custVM.FirstOrDefault(x => x.CustomerId == cus.CustomerID);
                            if (entity != null)
                            {
                                entity.IsOptOut = isOptOut;
                                entity.TaxPassword = cus.IncomeTaxPassword;
                                entity.IsProcessed = false;
                                entity.AsOfDate = request.customers.AsOfDate;
                                _context.CustomerTaxLogin.Update(entity);
                                await _context.SaveChangesAsync(cancellationToken);
                            }
                            else {
                                var newEntity = new CustomerTaxLogin
                                {
                                    CustomerId = cus.CustomerID,
                                    UnitNo = unitNo,
                                    TaxPassword = cus.IncomeTaxPassword,
                                    IsOptOut = isOptOut,
                                    IsProcessed = false,
                                    AsOfDate = request.customers.AsOfDate
                                };
                                await _context.CustomerTaxLogin.AddAsync(newEntity, cancellationToken);
                                await _context.SaveChangesAsync(cancellationToken);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex) {
                    throw ex;
                }

              
            }
        }
    }
}
