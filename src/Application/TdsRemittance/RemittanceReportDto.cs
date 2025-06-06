﻿using System;
using AutoMapper;
using ReProServices.Application.Common.Mappings;
using ReProServices.Domain.Entities;

namespace ReProServices.Application.TdsRemittance
{
    public class RemittanceReportDto
    {
        public int RemittanceID { get; set; }
        public int ClientPaymentTransactionID { get; set; }
        public string ChallanID { get; set; }
        public DateTime ChallanDate { get; set; }
        public string ChallanAckNo { get; set; }
        public decimal ChallanAmount { get; set; }
        public DateTime? F16BDateOfReq { get; set; }
        public string F16BRequestNo { get; set; }
        public string F16BCertificateNo { get; set; }
        public virtual int RemittanceStatusID { get; set; }
        public int? Form16BlobID { get; set; }
        public int? ChallanBlobID { get; set; }
        public string F16CustName { get; set; }
        public DateTime? F16UpdateDate { get; set; }
        public decimal? F16CreditedAmount { get; set; }
        public bool? EmailSent { get; set; } = false;
        public DateTime? EmailSentDate { get; set; }
        public string ChallanCustomerName { get; set; }
        public decimal? ChallanIncomeTaxAmount { get; set; }
        public decimal? ChallanInterestAmount { get; set; }
        public decimal? ChallanFeeAmount { get; set; }
        public virtual string UnitNo { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual string Premises { get; set; }
        public virtual int LotNo { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual string CustomerPAN { get; set; }

        public DateTime DateOfPayment { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal? GST{ get; set; }
        public decimal? TdsRate { get; set; }
    }
}
