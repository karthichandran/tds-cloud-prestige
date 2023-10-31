using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using ReProServices.Application.Common.Interfaces;

namespace ReProServices.Application.DetailsSummaryReport.Query
{
    public class GetDetailsSummaryReportQuery : IRequest<IList<DetailsSummaryReportDto>>
    {
        public int lotNo { get; set; }
        public class GetDetailsSummaryReportQueryHandler :
                              IRequestHandler<GetDetailsSummaryReportQuery, IList<DetailsSummaryReportDto>>
        {

            private readonly IApplicationDbContext _context;

            public GetDetailsSummaryReportQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<DetailsSummaryReportDto>> Handle(GetDetailsSummaryReportQuery request, CancellationToken cancellationToken)
            {

                try
                {
                    var qry = "exec sp_DetailSummary " + request.lotNo;                 

                    var vm = _context.DetailsSummaryReports.FromSqlRaw(qry).ToList();
                    var vmFinal = vm.Select((x, index) =>
                             new DetailsSummaryReportDto{ 
                                 SerialNo=index,
                                 LotNo=x.LotNo,
                                 AddressPremises=x.AddressPremises,
                                 TotalPayment=x.TotalPayment,
                                 Tds=x.Tds,
                                 TdsPaid=x.TdsPaid,
                                 DACompleted=x.DACompleted,
                                 DAPending=x.DAPending,
                                 F16bRequested=x.F16bRequested,
                                 F16bDownloaded=x.F16bDownloaded,
                                 F16bEmailed=x.F16bEmailed,
                                 OnlyTds=x.OnlyTds,
                                 Pending=x.Pending,
                                 Resolved=x.Resolved
                             }).OrderByDescending(x => x.LotNo)
                            .ThenBy(_ => _.AddressPremises)
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
