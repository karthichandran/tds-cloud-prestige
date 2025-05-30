using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReProServices.Application.DetailsSummaryReport;
using ReProServices.Application.DetailsSummaryReport.Query;
using ReProServices.Application.Form16BStatusSummary;
using ReProServices.Application.TdsPaymentSummaryReport;
using ReProServices.Application.TdsRemittance.Queries;
using ReProServices.Application.TdsRemittance.Queries.GetRemittanceList;
using ReProServices.Domain.Entities;
using WeihanLi.Npoi;

namespace WebApi.Controllers
{
    public class Form16BStatusSummaryController : ApiController
    {

        [HttpGet]
        public async Task<IList<Form16BStatusSummaryReportDto>> GetTdsPaypment([FromQuery] TdsPaymentFilter tdsFilter)
        {
            return await Mediator.Send(new Form16BSummaryQuery() { Filter = tdsFilter });
        }
        [HttpGet("getExcel")]
        public async Task<FileResult> GetReport([FromQuery] TdsPaymentFilter tdsFilter)
        {
            var result = await Mediator.Send(new Form16BSummaryQuery() { Filter = tdsFilter });
            var settings = FluentSettings.For<Form16BStatusSummaryReportDto>();
            settings.HasAuthor("REpro Services");
            settings.HasFreezePane(0, 1);
            settings.HasSheetConfiguration(0, "sheet 1", 1, true);

            settings.Property(x => x.LotNo)
                   .HasColumnTitle("Lot No")
                   .HasColumnIndex(0);

            settings.Property(x => x.ExpectedPaymentDate)
                    .HasColumnTitle("Date")
                    .HasColumnFormatter("dd-MMM-yyy")
                    .HasColumnIndex(1);

            //settings.Property(x => x.TotalPaymentWithoutRemark)
            //        .HasColumnTitle("Total No.of Payments without Remarks")
            //        .HasColumnIndex(2);

            settings.Property(x => x.NoOfCompleted)
                  .HasColumnTitle("Completed No.of payments")
                  .HasColumnIndex(2);

            settings.Property(x => x.NoOfPendingChallanDownload)
                .HasColumnTitle("No.of Pending challan download ")
                .HasColumnIndex(3);

            settings.Property(x => x.NoOfChallanDownloaded)
                    .HasColumnTitle("No.of Challan downloaded")
                    .HasColumnIndex(4);

            settings.Property(x => x.NoOfForm16BReq)
                  .HasColumnTitle("No.of Form 16B Requested")
                  .HasColumnIndex(5);

            settings.Property(x => x.NoOfForm16BDownloaded)
                  .HasColumnTitle("No.of Form 16B Downloaded")
                  .HasColumnIndex(6);

            settings.Property(x => x.PendingWithRemarks)
                    .HasColumnTitle("Pending with Remarks")
                    .HasColumnIndex(7);

            settings.Property(_ => _.TotalPaymentWithoutRemark).Ignored();

            var ms = result.ToExcelBytes();

            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "From16B_Status_summary_Report.xls");

        }

    }
}
