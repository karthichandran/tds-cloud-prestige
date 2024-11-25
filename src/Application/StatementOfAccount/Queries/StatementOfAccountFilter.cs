using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.StatementOfAccount.Queries
{
   public class StatementOfAccountFilter
    {
        public string CustomerName { get; set; }
        public int? PropertyId { get; set; }
        public string UnitNo { get; set; }
    }
}
