using MediatR;
using ReProServices.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReProServices.Application.CustonerTaxLogin.Queries
{
    public class GetCustomerTaxPasswordQuery :   IRequest<List<CustomerTaxPasswordDto>>
    {
        public int unitNo { get; set; }

        public class GetCustomerTaxPasswordQueryHandler : IRequestHandler<GetCustomerTaxPasswordQuery, List<CustomerTaxPasswordDto>>
        {
            private readonly IApplicationDbContext _context;

            public GetCustomerTaxPasswordQueryHandler(IApplicationDbContext context)
            {
                _context = context;

            }

            public async Task<List<CustomerTaxPasswordDto>> Handle(GetCustomerTaxPasswordQuery request, CancellationToken cancellationToken)
            {


                try
                {

                    var model = new List<CustomerTaxPasswordDto>();
                    if (request.unitNo > 0)
                        model = (from tp in _context.CustomerTaxLogin
                                 join cs in _context.Customer on tp.CustomerId equals cs.CustomerID
                                 where tp.IsProcessed == false && tp.UnitNo==request.unitNo
                                 select (new CustomerTaxPasswordDto
                                 {
                                     CustomerTaxLoginId = tp.CustomerTaxLoginId,
                                     CustomerId = tp.CustomerId,
                                     Name = cs.Name,
                                     UnitNo = tp.UnitNo.Value,
                                     TaxPassword = tp.TaxPassword,
                                     IsOptOut = tp.IsOptOut.Value,
                                     Pan=cs.PAN

                                 })).ToList();
                    else
                        model = (from tp in _context.CustomerTaxLogin
                                 join cs in _context.Customer on tp.CustomerId equals cs.CustomerID
                                 where tp.IsProcessed==false
                                 select (new CustomerTaxPasswordDto
                                 {
                                     CustomerTaxLoginId = tp.CustomerTaxLoginId,
                                     CustomerId = tp.CustomerId,
                                     Name = cs.Name,
                                     UnitNo = tp.UnitNo.Value,
                                     TaxPassword = tp.TaxPassword,
                                     IsOptOut = tp.IsOptOut.Value,
                                     Pan = cs.PAN

                                 })).ToList();

                    return model;

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
