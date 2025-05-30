using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReProServices.Application.DetailsSummaryReport;
using ReProServices.Application.DetailsSummaryReport.Query;
using ReProServices.Application.TdsPaymentSummaryReport;
using ReProServices.Application.TdsRemittance.Queries;
using ReProServices.Application.TdsRemittance.Queries.GetRemittanceList;
using ReProServices.Domain.Entities;
using WeihanLi.Npoi;

namespace WebApi.Controllers
{
    public class TdsPaymentSummaryReportController : ApiController
    {

        [HttpGet]
        public async Task<IList<TdsPaymentSummaryReportDto>> GetTdsPaypment([FromQuery] TdsPaymentFilter tdsFilter)
        {
            return await Mediator.Send(new TdsPaymentSummaryQuery() { Filter = tdsFilter });
        }
        [HttpGet("getExcel")]
        public async Task<FileResult> GetReport([FromQuery] TdsPaymentFilter tdsFilter)
        {
            var result = await Mediator.Send(new TdsPaymentSummaryQuery() { Filter = tdsFilter });
            var settings = FluentSettings.For<TdsPaymentSummaryReportDto>();
            settings.HasAuthor("REpro Services");
            settings.HasFreezePane(0, 1);
            settings.HasSheetConfiguration(0, "sheet 1", 1, true);

            settings.Property(x => x.LotNo)
                   .HasColumnTitle("Lot No")
                   .HasColumnIndex(0);

            settings.Property(x => x.ExpectedPaymentDate)
                    .HasColumnTitle("Expected Date of Payment ")
                    .HasColumnFormatter("dd-MMM-yyy")
                    .HasColumnIndex(1);

            settings.Property(x => x.TotalPayment)
                    .HasColumnTitle("Total No.of Payments")
                    .HasColumnIndex(2);

            settings.Property(x => x.TDSamount)
                  .HasColumnTitle("TDS Amount")
                  .HasColumnIndex(3);

            settings.Property(x => x.PaymentBy)
                    .HasColumnTitle("User")
                    .HasColumnIndex(4);

            settings.Property(x => x.CompletedPayment)
                  .HasColumnTitle("Completed No.of Payments")
                  .HasColumnIndex(5);

            settings.Property(x => x.CompletedPaymentTDS)
                  .HasColumnTitle("Completed payments TDS value")
                  .HasColumnIndex(6);

            settings.Property(x => x.PendingPaymentWithRemark)
                    .HasColumnTitle("Pending with Remarks")
                    .HasColumnIndex(7);

            settings.Property(x => x.PendingPaymentWithRemarkTDS)
                    .HasColumnTitle("Pending payments TDS value with Remarks")
                    .HasColumnIndex(8);

            settings.Property(x => x.PendingPaymentWithoutRemark)
                    .HasColumnTitle("Pending without Remarks")
                    .HasColumnIndex(9);

            settings.Property(x => x.PendingPaymentWithoutRemarkTDS)
                    .HasColumnTitle("Pending payments TDS value without Remarks")
                    .HasColumnIndex(10);

          

            var ms = result.ToExcelBytes();

            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TDS_Payments_Summary_Report.xls");

        }

    }
}
