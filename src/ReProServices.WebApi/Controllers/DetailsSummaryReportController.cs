using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReProServices.Application.DetailsSummaryReport;
using ReProServices.Application.DetailsSummaryReport.Query;
using WeihanLi.Npoi;

namespace WebApi.Controllers
{
    public class DetailsSummaryReportController : ApiController
    {
        [HttpGet("{lotno}")]
        public async Task<IList<DetailsSummaryReportDto>> Get(int lotno)
        {
            return await Mediator.Send(new GetDetailsSummaryReportQuery() { lotNo=lotno});
        }

        [HttpGet("getExcel/{lotno}")]
        public async Task<FileResult> GetReport(int lotno)
        {
            var result = await Mediator.Send(new GetDetailsSummaryReportQuery() { lotNo = lotno });
            var settings = FluentSettings.For<DetailsSummaryReportDto>();
            settings.HasAuthor("REpro Services");
            settings.HasFreezePane(0, 1);
            settings.HasSheetConfiguration(0, "sheet 1", 1, true);

            settings.Property(x => x.SerialNo)
                   .HasColumnTitle("S.No")
                   .HasColumnIndex(0);

            settings.Property(x => x.LotNo)
                    .HasColumnTitle("Lot No.")
                    .HasColumnIndex(1);

            settings.Property(x => x.AddressPremises)
                    .HasColumnTitle("Project Name")
                    .HasColumnIndex(2);

            settings.Property(x => x.TotalPayment)
                  .HasColumnTitle("No. of Client Payment")
                  .HasColumnIndex(3);

            settings.Property(x => x.Tds)
                    .HasColumnTitle("Total TDS value")
                    .HasColumnIndex(4);           

            settings.Property(x => x.TdsPaid)
                  .HasColumnTitle("Total TDS value paid")
                  .HasColumnIndex(5);

            settings.Property(x => x.DACompleted)
                  .HasColumnTitle("DA True")
                  .HasColumnIndex(6);

            settings.Property(x => x.DAPending)
                    .HasColumnTitle("DA False")
                    .HasColumnIndex(7);

            settings.Property(x => x.F16bRequested)
                    .HasColumnTitle("Form 16B Requested")
                    .HasColumnIndex(8);

            settings.Property(x => x.F16bDownloaded)
                    .HasColumnTitle("Form 16B Downloaded")
                    .HasColumnIndex(9);

            settings.Property(x => x.F16bEmailed)
                    .HasColumnTitle("Form 16B sent to customers")
                    .HasColumnIndex(10);

            settings.Property(x => x.OnlyTds)
                    .HasColumnTitle("Only TDS Payment")
                    .HasColumnIndex(11);

            settings.Property(x => x.Pending)
                    .HasColumnTitle("Pending Records")
                    .HasColumnIndex(12); ;

            settings.Property(x => x.Resolved)
                    .HasColumnTitle("Resolved Records")
                    .HasColumnIndex(13);

            var ms = result.ToExcelBytes();

            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DetailSummaryReport.xls");

        }

    }
}
