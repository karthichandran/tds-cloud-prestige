using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Domain.Enums;

namespace ReProServices.Application.TdsRemittance.Queries.GetRemittanceList
{
   public class GetFor16bblobQuery : IRequest<IList<int>>
    {
        public int LotNo  { get; set; }
        public int proeprtyID { get; set; }
        public class GetFor16bblobQueryHandler : IRequestHandler<GetFor16bblobQuery, IList<int>>
        {
            private readonly IApplicationDbContext _context;

            public GetFor16bblobQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<int>> Handle(GetFor16bblobQuery request, CancellationToken cancellationToken)
            {
                var remittances =await (from pay in _context.ClientPayment
                                   join cpt in _context.ClientPaymentTransaction on pay.ClientPaymentID equals cpt.ClientPaymentID
                                   join cp in _context.ViewCustomerPropertyExpanded on new { cpt.OwnershipID, cpt.CustomerID } equals new { cp.OwnershipID, cp.CustomerID }
                                   join r in _context.Remittance on cpt.ClientPaymentTransactionID equals r.ClientPaymentTransactionID
                                   where pay.LotNo==request.LotNo && cp.PropertyID==request.proeprtyID && r.Form16BlobID!=null

                                        select r.Form16BlobID.Value)
                    
                    .ToListAsync<int>();
                return  remittances;
            }

        }
    }
}
