using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Domain.Enums;

namespace ReProServices.Application.TdsRemittance.Queries.GetRemittanceList
{
    public class GetTdsPendingRemittanceListQuery : IRequest<IList<TdsRemittanceDto>>
    {
        public TdsRemittanceFilter Filter { get; set; }
        public class GetTdsPendingRemittanceListQueryHandler : IRequestHandler<GetTdsPendingRemittanceListQuery, IList<TdsRemittanceDto>>
        {
            private readonly IApplicationDbContext _context;

            public GetTdsPendingRemittanceListQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<TdsRemittanceDto>> Handle(GetTdsPendingRemittanceListQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter;
                List<string> unitNos = (from cus in _context.Customer join cp in _context.CustomerProperty on cus.CustomerID equals cp.CustomerId where cus.InvalidPAN == true select  cp.UnitNo).ToList();

                List<string> filteredUnitNo = new List<string>();
                if (!string.IsNullOrEmpty(filter.FromUnitNo) && !string.IsNullOrEmpty(filter.ToUnitNo)){
                    int fromUnit = Convert.ToInt32(filter.FromUnitNo);
                int toUnit = Convert.ToInt32(filter.ToUnitNo);
                 filteredUnitNo = _context.CustomerProperty.AsQueryable().Select(x => x.UnitNo).ToList().Where(cp => FilterUnitNo(cp, fromUnit, toUnit)).Distinct().ToList();
                }

                var remittances = (from pay in _context.ClientPayment
                   join cpt in _context.ClientPaymentTransaction on pay.ClientPaymentID equals cpt.ClientPaymentID
                   join cp in _context.ViewCustomerPropertyExpanded on new {cpt.OwnershipID, cpt.CustomerID} equals new
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
                   where cpt.RemittanceStatusID == (int) ERemittanceStatus.Pending
                         && pay.NatureOfPaymentID == (int) ENatureOfPayment.ToBeConsidered
                         && cpt.SellerID == sp.SellerID && cp.StatusTypeID != 3 && cp.InvalidPAN != true &&
                         cp.LessThan50L != true && cp.CustomerOptedOut != true
                         // && !ownershipIds.Contains(cp.OwnershipID)
                         && !unitNos.Contains(cp.UnitNo) &&(filteredUnitNo.Count==0|| filteredUnitNo.Contains(cp.UnitNo)) &&
                         (ctrOut.TracesRemarkId == 0 || ctrOut.TracesRemarkId == null)
                   //for presstige only
                   select new TdsRemittanceDto
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
                       TransactionLog = tlOut.Comment
                   }).Distinct()

                    //var filtered= remittances.Where(_ => string.Compare(_.UnitNo.Trim(), request.Filter.FromUnitNo.ToString()) >= 0 && string.Compare(_.UnitNo.Trim(), request.Filter.FromUnitNo.ToString()) <= 0).ToList();
                    //    remittances
                    .PreFilterRemittanceBy(request.Filter)
                    .ToList()
                    .AsQueryable()
                    .PostFilterRemittanceBy(request.Filter)
                    .ToList();
                return remittances;
            }


            private static bool FilterUnitNo(string unitNo,int from,int to)
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
