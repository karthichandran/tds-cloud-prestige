using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.Traces
{
    public class TracesPasswordSettingDto
    {
        public int ClientPaymentTransactionID { get; set; }
        public Guid OwnershipID { get; set; }
        public string PropertyPremises { get; set; }
        public string UnitNo { get; set; }
     
        public int LotNo { get; set; }
        public decimal AmountPaid { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ChallanCustomerName { get; set; }
        public  DateTime ChallanDate { get; set; }
        public  string ChallanAckNo { get; set; }
        public string ChallanSlNo { get; set; }
        public  decimal ChallanAmount { get; set; }
        public  string CustomerPAN { get; set; }
     
        public  string TracesPassword { get; set; }
        public string PaymentBy { get; set; }
        public  DateTime DateOfBirth { get; set; }
        public int NoOfAttempts { get; set; }
        public DateTime? DebitAdviceCreated { get; set; }
        public int? RemarkId { get; set; }
        public string RemarkDesc { get; set; }
        public bool OnlyTds { get; set; }
    }
}
