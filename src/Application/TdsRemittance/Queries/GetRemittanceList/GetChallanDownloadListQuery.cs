using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Domain.Enums;

namespace ReProServices.Application.TdsRemittance.Queries.GetRemittanceList
{
    public class GetChallanDownloadListQuery : IRequest<IList<ChallanDownloadDto>>
    {
        public TdsPaymentFilter Filter { get; set; }

        public class GetChallanDownloadListQueryHandler :
            IRequestHandler<GetChallanDownloadListQuery, IList<ChallanDownloadDto>>
        {

            private readonly IApplicationDbContext _context;

            public GetChallanDownloadListQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<ChallanDownloadDto>> Handle(GetChallanDownloadListQuery request,
                CancellationToken cancellationToken)
            {
                var filter = request.Filter;


                var remittances = (from pay in _context.ClientPayment
                                   join cpt in _context.ClientPaymentTransaction on pay.ClientPaymentID equals cpt.ClientPaymentID
                                   join cp in _context.ViewCustomerPropertyExpanded on new { cpt.OwnershipID, cpt.CustomerID } equals new
                                   { cp.OwnershipID, cp.CustomerID }
                                   join sp in _context.ViewSellerPropertyExpanded on cp.PropertyID equals sp.PropertyID
                                   join da in _context.DebitAdvices on cpt.ClientPaymentTransactionID equals da
                                       .ClientPaymentTransactionID into xObj
                                   from dam in xObj.DefaultIfEmpty()
                                   join r in _context.Remittance on cpt.ClientPaymentTransactionID equals r.ClientPaymentTransactionID  into rout
                                   from rt in rout.DefaultIfEmpty()
                                   join remSt in _context.RemittanceStatus on cpt.RemittanceStatusID equals remSt.RemittanceStatusID
                                   join ctr in _context.ClientTransactionRemark on cpt.ClientPaymentTransactionID equals ctr
                                       .ClientPaymentTransactionId into clObj
                                   from ctrOut in clObj.DefaultIfEmpty()
                                   join rm in _context.RemittanceRemark on ctrOut.RemittanceRemarkId equals rm.RemarkId into rmObj
                                   from rmOut in rmObj.DefaultIfEmpty()
                                    where (request.Filter.RemittanceStatusID.HasValue) || cpt.RemittanceStatusID <= (int) ERemittanceStatus.TdsPaid &&
                                    cpt.SellerID == sp.SellerID &&
                                         dam!=null &&
                                       ( rt==null || rt.ChallanDate==null)&&
                                         (string.IsNullOrEmpty(filter.UnitNo) || cp.UnitNo.Contains(filter.UnitNo)) &&
                                         (filter.LotNo == 0 || filter.LotNo == pay.LotNo) &&
                                         (string.IsNullOrEmpty(filter.PaymentBy) || cpt.PaymentBy.Contains(filter.PaymentBy)) &&
                                         (filter.ExpectedFromDate == null || filter.ExpectedToDate == null ||
                                          cpt.ExpectedPaymentDate >= filter.ExpectedFromDate &&
                                          cpt.ExpectedPaymentDate <= filter.ExpectedToDate) &&
                                         (filter.PropertyId == null || cp.PropertyID == filter.PropertyId) &&
                                         (string.IsNullOrEmpty(filter.CustomerName) || cp.CustomerName.Contains(filter.CustomerName))
                                         && (filter.FromTransNo == 0 || filter.ToTransNo == 0 || (cpt.ClientPaymentTransactionID >= filter.FromTransNo && cpt.ClientPaymentTransactionID <= filter.ToTransNo))
                                   select new ChallanDownloadDto
                                   {
                                       ClientPaymentTransactionID = cpt.ClientPaymentTransactionID,
                                       CustomerName = cp.CustomerName,
                                       CustomerShare = cpt.CustomerShare,
                                       SellerName = sp.SellerName,
                                       SellerShare = cpt.SellerShare,
                                       PropertyPremises = sp.PropertyPremises,
                                       UnitNo = cp.UnitNo,
                                       TdsCollectedBySeller = cp.TdsCollectedBySeller,
                                       OwnershipID = cp.OwnershipID,
                                       InstallmentID = pay.InstallmentID,
                                       GstAmount = cpt.Gst,
                                       RevisedDateOfPayment = pay.RevisedDateOfPayment,
                                       DateOfDeduction = pay.DateOfDeduction,
                                       ReceiptNo = pay.ReceiptNo,
                                       ClientPaymentID = pay.ClientPaymentID,
                                       LotNo = pay.LotNo,
                                       GrossShareAmount = cpt.GrossShareAmount,
                                       AmountShare = cpt.ShareAmount,
                                       RemittanceStatus = remSt.RemittanceStatusText,
                                       TdsInterest = cpt.TdsInterest,
                                       LateFee = cpt.LateFee,
                                       TdsAmount = cpt.Tds,
                                       RemittanceStatusID = cpt.RemittanceStatusID,
                                       CustomerPAN = cp.CustomerPAN,
                                       RemarkId = rmOut.RemarkId,
                                       RemarkDesc = rmOut.Description,
                                       TracesPassword = cp.TracesPassword,
                                       PaymentBy = cpt.PaymentBy,
                                       PaymentDate=dam.PaymentDate,
                                       DebitAdviceCreated = dam.Created,
                                       NoOfAttempts = cpt.NoOfAttempst ?? 0
                                   }).ToList();
                return remittances;
            }



        }

    }
}
