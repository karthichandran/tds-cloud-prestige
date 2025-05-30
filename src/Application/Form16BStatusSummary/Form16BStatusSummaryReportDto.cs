using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.Form16BStatusSummary
{
   public class Form16BStatusSummaryReportDto
    {
        public int? LotNo { get; set; }
        public DateTime? ExpectedPaymentDate { get; set; }
        public int? TotalPaymentWithoutRemark { get; set; }
        public int? NoOfCompleted { get; set; }
        public int? NoOfPendingChallanDownload { get; set; }
        public int? NoOfChallanDownloaded { get; set; }
        public int? NoOfForm16BReq { get; set; }
        public int? NoOfForm16BDownloaded { get; set; }
        public int? PendingWithRemarks { get; set; }
    }
}
