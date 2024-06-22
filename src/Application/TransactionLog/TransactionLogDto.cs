using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.TransactionLog
{
    public class TransactionLogDto
    {
        public int LogId { get; set; }
        public int ClientPaymentTransactionId { get; set; }
        public string Comment { get; set; }
        public string ChalanDownload { get; set; }
    }
}
