using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReProServices.Domain.Entities
{
    public class TransactionLog
    {
        [Key]
        public int LogId { get; set; }
        public int ClientPaymentTransactionId { get; set; }
        public string Comment { get; set; }
        public string ChalanDownload { get; set; }
    }
}
