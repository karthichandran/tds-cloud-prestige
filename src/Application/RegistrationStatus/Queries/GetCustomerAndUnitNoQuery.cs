using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;

namespace ReProServices.Application.RegistrationStatus.Queries
{
   public class GetCustomerAndUnitNoQuery : IRequest<CustomerUnitNoModel>
    {
        public int ProjectId { get; set; }

        public class GetCustomerAndUnitNoQueryHandler : IRequestHandler<GetCustomerAndUnitNoQuery, CustomerUnitNoModel>
        {
            private readonly IApplicationDbContext _context;

            public GetCustomerAndUnitNoQueryHandler(IApplicationDbContext context, IClientPortalDbContext portContext)
            {
                _context = context;
            }

            public async Task<CustomerUnitNoModel> Handle(GetCustomerAndUnitNoQuery request, CancellationToken cancellationToken)
            {
                var customerList =await (from cp in _context.CustomerProperty
                        join cs in _context.Customer on cp.CustomerId equals cs.CustomerID
                        where cp.PropertyId == request.ProjectId
                        select new DropdownModel() {Id = cs.CustomerID, Description = cs.Name})
                    .ToListAsync(cancellationToken);


                var unitNoList =await  _context.CustomerProperty.Where(x => x.PropertyId == request.ProjectId)
                    .Select(s => new DropdownModel() {Id = s.CustomerId, Description = s.UnitNo})
                    .ToListAsync(cancellationToken);


                var model = new CustomerUnitNoModel()
                {
                    CustomerName = customerList,
                    UnitNo = unitNoList
                };
                return model;
            }

        }
    }
}
