using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ReProServices.Application.CustonerTaxLogin.Queries
{
    public class GetCustomersByUnitNoQuery : IRequest<CustomerTaxLoginDetails>
    {
        public int UnitNo { get; set; }
        public int PropertyId { get; set; }
        public class GetCustomersByUnitNoQueryHandler : IRequestHandler<GetCustomersByUnitNoQuery, CustomerTaxLoginDetails>
        {
            private readonly IApplicationDbContext _context;

            public GetCustomersByUnitNoQueryHandler(IApplicationDbContext context)
            {
                _context = context;

            }

            public async Task<CustomerTaxLoginDetails> Handle(GetCustomersByUnitNoQuery request, CancellationToken cancellationToken)
            {


                try
                {
                    var custDto = await (from cus in _context.Customer
                                   join cp in _context.CustomerProperty on cus.CustomerID equals cp.CustomerId
                                   where cp.PropertyId== request.PropertyId &&  cp.UnitNo == request.UnitNo 
                                   select new CustomerDto { CustomerID = cus.CustomerID,Name=cus.Name,PAN=cus.PAN,IncomeTaxPassword=cus.IncomeTaxPassword }).ToListAsync<CustomerDto>();


                    var cusModel= new CustomerTaxLoginDetails
                    {
                        customers = custDto,
                        UnitNo=request.UnitNo
                    };

                    return cusModel;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


            }
        }
    }
}
