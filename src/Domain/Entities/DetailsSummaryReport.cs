using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReProServices.Domain.Entities
{
   [Table("DetailsSummaryReport")]
    public class DetailsSummaryReport
    {
        public int LotNo { get; set; }
        public string AddressPremises { get; set; }
        public int TotalPayment { get; set; }
        public decimal Tds { get; set; }
        public decimal TdsPaid { get; set; }
        public int DACompleted { get; set; }
        public int DAPending { get; set; }
        public int F16bRequested { get; set; }
        public int F16bDownloaded { get; set; }
        public int F16bEmailed { get; set; }
        public int OnlyTds { get; set; }
        public int Pending { get; set; }
        public int Resolved { get; set; }
    }
}
