using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;

namespace ReProServices.Application.SellerComplianceReport.Queries
{
   public class GetSellerComplianceReportQuery : IRequest<IList<SellerComplianceDto>>
    {
        public SellerComplianceReportFilter Filter { get; set; } = new SellerComplianceReportFilter();

        public class GetSellerComplianceReportQueryHandler : IRequestHandler<GetSellerComplianceReportQuery, IList<SellerComplianceDto>> {
            private readonly IApplicationDbContext _context;
            public GetSellerComplianceReportQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<SellerComplianceDto>> Handle(GetSellerComplianceReportQuery request, CancellationToken cancellationToken)
            {
                

                var vm = (from cp in _context.ViewCustomerPropertyBasic
                          join cpt in _context.ClientPaymentTransaction on new { cp.OwnershipID, cp.CustomerID } equals new { cpt.OwnershipID, cpt.CustomerID }
                          join pay in _context.ClientPayment on cpt.ClientPaymentID equals pay.ClientPaymentID
                          join sl in _context.ViewSellerPropertyBasic on cp.PropertyID equals sl.PropertyID
                          join rt in _context.Remittance on cpt.ClientPaymentTransactionID equals rt.ClientPaymentTransactionID into rmGrp
                         from rt in rmGrp.DefaultIfEmpty()
                          join f in _context.ViewCustomerPropertyFile on rt.Form16BlobID equals f.BlobID  into fGrp
                          from f in fGrp.DefaultIfEmpty()
                          select new SellerComplianceDto {
                              SellerID=sl.SellerID,
                              SellerName=sl.SellerName,
                              PropertyID = cp.PropertyID,
                              CustomerName = cp.CustomerName,
                              Premises = cp.PropertyPremises,
                              UnitNo = cp.UnitNo,
                              LotNo = pay.LotNo,
                              TdsCertificateDate=rt.F16UpdateDate,
                              TdsCertificateNo=rt.F16BCertificateNo,
                              Amount=rt.F16CreditedAmount,
                              Form16BFileName=f.FileName,
                              // CustomerNo = pay.CustomerNo,
                              CustomerNo =cp.CustomerNo,
                              PropertyCode = sl.PropertyCode,
                              TransactionId = cpt.ClientPaymentTransactionID,
                              Material = pay.Material,
                              TaxDepositDate = rt.ChallanDate,
                              AssessmentYear = AssessYear(f.FileName),
                              ChallanAmount = rt.ChallanAmount,
                              AckNo = rt.ChallanAckNo
                          }
                          ).PreFilterReportBy(request.Filter)
                        .ToList()
                        .AsQueryable()
                        .PostFilterReportBy(request.Filter)
                        .ToList();
                return vm;
            }

            private static string AssessYear(string fileName)
            {
                if( string.IsNullOrEmpty(fileName))
                    return string.Empty;

                var splited = fileName.Split('_')[1];
                return splited;
            }
        }
    }
}
