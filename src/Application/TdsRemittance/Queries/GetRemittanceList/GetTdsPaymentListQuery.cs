using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Domain.Enums;

namespace ReProServices.Application.TdsRemittance.Queries.GetRemittanceList
{
    public class GetTdsPaymentListQuery : IRequest<IList<TdsPaymentDto>>
    {
        public TdsPaymentFilter Filter { get; set; }

        public class GetTdsPaymentListQueryHandler : IRequestHandler<GetTdsPaymentListQuery,
                IList<TdsPaymentDto>>
        {
            private readonly IApplicationDbContext _context;

            public GetTdsPaymentListQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<TdsPaymentDto>> Handle(GetTdsPaymentListQuery request,
                CancellationToken cancellationToken)
            {
                var filter = request.Filter;

                var remittances = (from pay in _context.ClientPayment
                        join cpt in _context.ClientPaymentTransaction on pay.ClientPaymentID equals cpt.ClientPaymentID
                        join cp in _context.ViewCustomerPropertyExpanded on new {cpt.OwnershipID, cpt.CustomerID} equals
                            new
                                {cp.OwnershipID, cp.CustomerID}
                        join sp in _context.ViewSellerPropertyExpanded on cp.PropertyID equals sp.PropertyID
                        join da in _context.DebitAdvices on cpt.ClientPaymentTransactionID equals da
                            .ClientPaymentTransactionID into xObj
                        from dam in xObj.DefaultIfEmpty()
                        join ctr in _context.ClientTransactionRemark on cpt.ClientPaymentTransactionID equals ctr
                            .ClientPaymentTransactionId into clObj
                        from ctrOut in clObj.DefaultIfEmpty()
                        join rm in _context.RemittanceRemark on ctrOut.RemittanceRemarkId equals rm.RemarkId into rmObj
                        from rmOut in rmObj.DefaultIfEmpty()
                        join tl in _context.TransactionLog on cpt.ClientPaymentTransactionID equals tl
                            .ClientPaymentTransactionId into tlObj
                        from tlOut in tlObj.DefaultIfEmpty()
                        join bn in _context.BankAccountDetails on cpt.BankAcctId equals bn.AccountId into bnObj
                        from bnOut in bnObj.DefaultIfEmpty()
                        where cpt.RemittanceStatusID == (int) ERemittanceStatus.Pending &&
                              pay.NatureOfPaymentID == (int) ENatureOfPayment.ToBeConsidered &&
                              cpt.SellerID == sp.SellerID &&
                              cp.StatusTypeID != 3 &&
                              //cp.CustomerOptedOut != true &&
                              (ctrOut.TracesRemarkId == 0 || ctrOut.TracesRemarkId == null) &&
                              dam==null&&
                              (string.IsNullOrEmpty(filter.UnitNo) || cp.UnitNo.Contains(filter.UnitNo)) &&
                              (filter.LotNo==0|| (filter.LotNo.ToString().Length<3? pay.LotNo== filter.LotNo :  pay.LotNo.ToString().Contains( filter.LotNo.ToString())) ) &&
                              (string.IsNullOrEmpty(filter.PaymentBy) || cpt.PaymentBy.Contains(filter.PaymentBy))&&
                              (filter.ExpectedFromDate==null || filter.ExpectedToDate==null || cpt.ExpectedPaymentDate>=filter.ExpectedFromDate && cpt.ExpectedPaymentDate<=filter.ExpectedToDate)&&
                              (filter.PropertyId==null || cp.PropertyID==filter.PropertyId) &&
                              (string.IsNullOrEmpty(filter.CustomerName) || cp.CustomerName.Contains(filter.CustomerName)) &&
                              (filter.FromTransNo==0 || filter.ToTransNo==0 || (cpt.ClientPaymentTransactionID>= filter.FromTransNo && cpt.ClientPaymentTransactionID <= filter.ToTransNo))
                        //for presstige only
                                   select new TdsPaymentDto
                        {
                            ClientPaymentTransactionID = cpt.ClientPaymentTransactionID,
                            CustomerName = cp.CustomerName,
                            CustomerShare = cpt.CustomerShare,
                            SellerName = sp.SellerName,
                            SellerShare = cpt.SellerShare,
                            SellerPAN = sp.SellerPAN,
                            PropertyPremises = sp.PropertyPremises,
                            UnitNo = cp.UnitNo,
                            TdsCollectedBySeller = cp.TdsCollectedBySeller,
                            OwnershipID = cp.OwnershipID,
                            InstallmentID = pay.InstallmentID,
                            StatusTypeID = cp.StatusTypeID,
                            GstAmount = cpt.Gst,
                            TdsInterest = cpt.TdsInterest,
                            AmountPaid = pay.AmountPaid,
                            GrossAmount = pay.GrossAmount,
                            RevisedDateOfPayment = pay.RevisedDateOfPayment,
                            DateOfDeduction = pay.DateOfDeduction,
                            ReceiptNo = pay.ReceiptNo,
                            LateFee = cpt.LateFee,
                            ClientPaymentID = pay.ClientPaymentID,
                            LotNo = pay.LotNo,
                            GrossShareAmount = cpt.GrossShareAmount,
                            TdsAmount = cpt.Tds,
                            AmountShare = cpt.ShareAmount,
                            RemittanceStatusID = cpt.RemittanceStatusID,
                            IsDebitAdvice = dam != null ? true : false,
                            RemarkId = rmOut.RemarkId,
                            RemarkDesc = rmOut.Description,
                            CinNo = dam.CinNo,
                            TransactionLog = tlOut.Comment,
                            PaymentBy = cpt.PaymentBy,
                            expectedPaymentDate = cpt.ExpectedPaymentDate,
                            Bank = bnOut.UserName,
                            BankAcctId = cpt.BankAcctId??0,
                            NoOfAttempts = cpt.NoOfAttempst??0,
                            IsResident = cp.NonResident!=true,
                            CustomerOptedOut = cp.CustomerOptedOut??false
                        }).Distinct()
                    .ToList();

                return remittances;
            }

            private static bool FilterUnitNo(string unitNo, int from, int to)
            {
                if (!unitNo.All(char.IsDigit))
                    return false;

                var num = Convert.ToInt32(unitNo);
                if (num >= from && num <= to)
                    return true;

                return false;
            }
        }
    }
}
