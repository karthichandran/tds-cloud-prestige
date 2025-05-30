using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.TdsRemittance.Queries
{
   public class TdsPaymentFilter
    {
        public string UnitNo { get; set; }
        public int? PropertyId { get; set; }
        public int LotNo { get; set; }
        public string CustomerName { get; set; }
        public string PaymentBy { get; set; }
        public int? RemittanceStatusID { get; set; }
        public DateTime? ExpectedFromDate { get; set; }
        public DateTime? ExpectedToDate { get; set; }
        public int FromTransNo { get; set; }
        public int ToTransNo { get; set; }
    }
}
