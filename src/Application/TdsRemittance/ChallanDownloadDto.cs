﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.TdsRemittance
{
    public class ChallanDownloadDto
    {
        public int ClientPaymentTransactionID { get; set; }
        public int ClientPaymentID { get; set; }
        public Guid OwnershipID { get; set; }
        public string PropertyPremises { get; set; }
        public string UnitNo { get; set; }
        public bool TdsCollectedBySeller { get; set; }
        public Guid InstallmentID { get; set; }
        public DateTime RevisedDateOfPayment { get; set; }
        public DateTime DateOfDeduction { get; set; }
        public string ReceiptNo { get; set; }
        public int LotNo { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal SellerShare { get; set; }
        public string SellerName { get; set; }
        public string CustomerName { get; set; }
        public decimal CustomerShare { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TdsAmount { get; set; }
        public decimal TdsInterest { get; set; }
        public decimal LateFee { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal OwnershipAmount { get; set; }
        public int StatusTypeID { get; set; }
        public decimal GrossShareAmount { get; set; }
        public int RemittanceStatusID { get; set; }
        public string RemittanceStatus { get; set; }
        public decimal AmountShare { get; set; }
        public virtual string CustomerPAN { get; set; }
        public virtual string SellerPAN { get; set; }
        public virtual string TracesPassword { get; set; }
        public virtual string AssessmentYear { get; set; }
        public int? RemarkId { get; set; }
        public string RemarkDesc { get; set; }
        public string CinNo { get; set; }
        public string TransactionLog { get; set; }
        public string PaymentBy { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? DebitAdviceCreated { get; set; }
        public int NoOfAttempts { get; set; }


    }
}
