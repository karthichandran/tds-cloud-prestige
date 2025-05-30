using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.TdsPaymentSummaryReport
{
   public class TdsPaymentSummaryReportDto
    {
        public int? LotNo { get; set; }
        public DateTime ExpectedPaymentDate { get; set; }

        public int? TotalPayment { get; set; }

        public int? CompletedPayment { get; set; }

        public decimal? CompletedPaymentTDS { get; set; }
        public int? PendingPaymentWithRemark { get; set; }

        public decimal? PendingPaymentWithRemarkTDS { get; set; }
        public int? PendingPaymentWithoutRemark { get; set; }

        public decimal? PendingPaymentWithoutRemarkTDS { get; set; }

        public decimal? TDSamount { get; set; }

        public string PaymentBy { get; set; }
    }
}
