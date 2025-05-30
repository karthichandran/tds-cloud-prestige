using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.TdsRemittance;
using ReProServices.Application.TdsRemittance.Queries;
using ReProServices.Application.TdsRemittance.Queries.GetRemittanceList;
using ReProServices.Domain.Enums;

namespace ReProServices.Application.Traces
{
    public class GetTracesPasswordSettingListQuery :IRequest<IList<TracesPasswordSettingDto>>
    {
        public TdsPaymentFilter Filter { get; set; }
        public class GetTracesPasswordSettingListQueryHandler :
                              IRequestHandler<GetTracesPasswordSettingListQuery, IList<TracesPasswordSettingDto>>
        {

            private readonly IApplicationDbContext _context;

            public GetTracesPasswordSettingListQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<TracesPasswordSettingDto>> Handle(GetTracesPasswordSettingListQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter;

                var remittances =await (from pay in _context.ClientPayment
                                   join cpt in _context.ClientPaymentTransaction on pay.ClientPaymentID equals cpt.ClientPaymentID
                                   join cp in _context.ViewCustomerPropertyExpanded on new { cpt.OwnershipID, cpt.CustomerID } equals new { cp.OwnershipID, cp.CustomerID }
                                   join sp in _context.ViewSellerPropertyExpanded on cp.PropertyID equals sp.PropertyID
                                   join da in _context.DebitAdvices on cpt.ClientPaymentTransactionID equals da
                                       .ClientPaymentTransactionID
                                        join rem in _context.Remittance on cpt.ClientPaymentTransactionID equals rem.ClientPaymentTransactionID
                                   join ctr in _context.ClientTransactionRemark on cpt.ClientPaymentTransactionID equals ctr
                                       .ClientPaymentTransactionId into clObj
                                   from ctrOut in clObj.DefaultIfEmpty()
                                   join rm in _context.RemittanceRemark on ctrOut.TracesRemarkId equals rm.RemarkId into rmObj
                                   from rmOut in rmObj.DefaultIfEmpty()
                                        where cpt.RemittanceStatusID == (int)ERemittanceStatus.TdsPaid
                                              && rem.F16BDateOfReq == null
                                              && rem.F16BRequestNo == null
                                              && cpt.SellerID == sp.SellerID
                                              && string.IsNullOrEmpty(cp.TracesPassword)
                                              && (string.IsNullOrEmpty(filter.UnitNo) || cp.UnitNo.Contains(filter.UnitNo)) &&
                                              (filter.LotNo == 0 || filter.LotNo == pay.LotNo) &&
                                              (string.IsNullOrEmpty(filter.PaymentBy) || cpt.PaymentBy.Contains(filter.PaymentBy)) &&
                                              (filter.ExpectedFromDate == null || filter.ExpectedToDate == null ||
                                               cpt.ExpectedPaymentDate >= filter.ExpectedFromDate &&
                                               cpt.ExpectedPaymentDate <= filter.ExpectedToDate) &&
                                              (filter.PropertyId == null || cp.PropertyID == filter.PropertyId) &&
                                              (string.IsNullOrEmpty(filter.CustomerName) || cp.CustomerName.Contains(filter.CustomerName))
                                              && (filter.FromTransNo == 0 || filter.ToTransNo == 0 || (cpt.ClientPaymentTransactionID >= filter.FromTransNo && cpt.ClientPaymentTransactionID <= filter.ToTransNo))
                                        
                                   select new TracesPasswordSettingDto
                                   {
                                       ClientPaymentTransactionID = cpt.ClientPaymentTransactionID,
                                       CustomerId = cp.CustomerID,
                                       CustomerName = cp.CustomerName,
                                       CustomerPAN = cp.CustomerPAN,
                                       DateOfBirth = cp.DateOfBirth,
                                     ChallanCustomerName = rem.ChallanCustomerName,
                                       PropertyPremises = sp.PropertyPremises,
                                       UnitNo = cp.UnitNo,
                                       OwnershipID = cp.OwnershipID,
                                       AmountPaid = pay.AmountPaid,
                                       LotNo = pay.LotNo,
                                       ChallanAmount = rem.ChallanAmount,
                                       ChallanDate = rem.ChallanDate,
                                       ChallanAckNo = rem.ChallanAckNo,
                                       ChallanSlNo=rem.ChallanID,
                                       DebitAdviceCreated = da.Created,
                                       RemarkId = rmOut.RemarkId,
                                       RemarkDesc = rmOut.Description,
                                       OnlyTds = cp.OnlyTDS??false,
                                       PaymentBy = cpt.PaymentBy
                                   }).ToListAsync();
                return remittances;
            }

        }
    }
}
