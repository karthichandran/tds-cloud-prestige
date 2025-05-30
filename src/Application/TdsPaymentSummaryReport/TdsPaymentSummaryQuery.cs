using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.DetailsSummaryReport;
using ReProServices.Application.DetailsSummaryReport.Query;
using ReProServices.Application.TdsRemittance.Queries;

namespace ReProServices.Application.TdsPaymentSummaryReport
{
    public class TdsPaymentSummaryQuery : IRequest<IList<TdsPaymentSummaryReportDto>>
    {
        public TdsPaymentFilter Filter { get; set; }
        public class TdsPaymentSummaryQueryHandler :
                              IRequestHandler<TdsPaymentSummaryQuery, IList<TdsPaymentSummaryReportDto>>
        {

            private readonly IApplicationDbContext _context;

            public TdsPaymentSummaryQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<TdsPaymentSummaryReportDto>> Handle(TdsPaymentSummaryQuery request, CancellationToken cancellationToken)
            {

                try
                {
                    var filter = request.Filter;
                    var qry = "exec usp_TdsPaymentsSummaryReport " + filter.LotNo + ",";
                    if (string.IsNullOrEmpty(filter.PaymentBy))
                        qry += "'',";
                    else
                        qry += "'"+filter.PaymentBy+"',";

                    if (filter.ExpectedFromDate==null)
                        qry += "'',";
                    else
                        qry += "'" + filter.ExpectedFromDate + "',";
                    if (filter.ExpectedToDate == null)
                        qry += "'',";
                    else
                        qry += "'" + filter.ExpectedToDate + "',";

                    if (filter.PropertyId == null)
                        qry += "0";
                    else
                        qry +=  + filter.PropertyId ;

                    var itm= _context.TdsPaymentSummaryReport.ToList();

                    var vm = _context.TdsPaymentSummaryReport.FromSqlRaw(qry).ToList();
                    var vmFinal = vm.Select((x, index) =>
                             new TdsPaymentSummaryReportDto
                             {
                                
                                 LotNo = x.LotNo,
                                 ExpectedPaymentDate = x.ExpectedPaymentDate,
                                 TotalPayment = x.TotalPayment,
                                 CompletedPayment = x.CompletedPayment,
                                 CompletedPaymentTDS = x.CompletedPaymentTDS,
                                 PendingPaymentWithRemark = x.PendingPaymentWithRemark,
                                 PendingPaymentWithRemarkTDS = x.PendingPaymentWithRemarkTDS,
                                 PendingPaymentWithoutRemark = x.PendingPaymentWithoutRemark,
                                 PendingPaymentWithoutRemarkTDS = x.PendingPaymentWithoutRemarkTDS,
                                 TDSamount = x.TDSamount,
                                 PaymentBy = x.PaymentBy,
                                
                             })
                            .ToList();
                    return vmFinal;
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
