using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.TdsPaymentSummaryReport;
using ReProServices.Application.TdsRemittance.Queries;

namespace ReProServices.Application.Form16BStatusSummary
{
    public class Form16BSummaryQuery : IRequest<IList<Form16BStatusSummaryReportDto>>
    {
        public TdsPaymentFilter Filter { get; set; }
        public class Form16BSummaryQueryHandler : IRequestHandler<Form16BSummaryQuery, IList<Form16BStatusSummaryReportDto>>
        {

            private readonly IApplicationDbContext _context;

            public Form16BSummaryQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<Form16BStatusSummaryReportDto>> Handle(Form16BSummaryQuery request, CancellationToken cancellationToken)
            {

                try
                {
                    var filter = request.Filter;
                    var qry = "exec usp_Form16BStatusSummaryReport " + filter.LotNo + ",";
                    if (string.IsNullOrEmpty(filter.PaymentBy))
                        qry += "'',";
                    else
                        qry += "'" + filter.PaymentBy + "',";

                    if (filter.ExpectedFromDate == null)
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
                        qry += +filter.PropertyId;

                  
                    var vm = _context.Form16BStatusSummaryReport.FromSqlRaw(qry).ToList();
                    var vmFinal = vm.Select((x, index) =>
                             new Form16BStatusSummaryReportDto
                             {
                                 LotNo = x.LotNo,
                                 ExpectedPaymentDate = x.ExpectedPaymentDate,
                                 NoOfCompleted = x.NoOfCompleted,
                                 NoOfPendingChallanDownload = x.NoOfPendingChallanDownload,
                                 TotalPaymentWithoutRemark = x.TotalPaymentWithoutRemark,
                                 NoOfChallanDownloaded = x.NoOfChallanDownloaded,
                                 NoOfForm16BReq = x.NoOfForm16BReq,
                                 NoOfForm16BDownloaded = x.NoOfForm16BDownloaded,
                                 PendingWithRemarks = x.PendingWithRemarks

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
