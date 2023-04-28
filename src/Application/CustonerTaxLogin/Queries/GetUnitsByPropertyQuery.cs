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
    public class GetUnitsByPropertyQuery : IRequest<List<UnitNumberDto>>
    {
        public int propertyId { get; set; }

        public class GetUnitsByPropertyQueryHandler : IRequestHandler<GetUnitsByPropertyQuery, List<UnitNumberDto>>
        {
            private readonly IApplicationDbContext _context;
           
            public GetUnitsByPropertyQueryHandler(IApplicationDbContext context )
            {
                _context = context;
                
            }

            public async Task<List<UnitNumberDto>> Handle(GetUnitsByPropertyQuery request, CancellationToken cancellationToken)
            {
               

                try
                {
                    //var model = _context.CustomerProperty.Where(x => x.PropertyId == request.propertyId).Select(x => new UnitNumberDto { UnitNo=x.UnitNo.ToString() }).Distinct().ToList<UnitNumberDto>();


                    var model = (from cp in _context.CustomerProperty
                                 join cus in _context.Customer on cp.CustomerId equals cus.CustomerID
                                 where (string.IsNullOrEmpty(cus.IncomeTaxPassword) && (cus.CustomerOptedOut==null ||cus.CustomerOptedOut.Value == false)) && cp.PropertyId == request.propertyId && cp.IsArchived.Value==false 
                                 select new UnitNumberDto { UnitNo = cp.UnitNo.ToString() }).Distinct().ToList();
                              


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
