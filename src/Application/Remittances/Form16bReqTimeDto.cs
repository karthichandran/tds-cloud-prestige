using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.Remittances
{
    public class Form16bReqTimeDto
    {
        public int ClientTransactionId { get; set; }
        public DateTime TimeOfReq { get; set; }
    }
}
