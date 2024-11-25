using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Domain.Entities
{
    public class CustomerTaxLogin
    {
        public int CustomerTaxLoginId { get; set; }
        public string UnitNo { get; set; }
        public int CustomerId { get; set; }
        public string TaxPassword { get; set; }
        public bool? IsOptOut { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? AsOfDate { get; set; }
    }
}
