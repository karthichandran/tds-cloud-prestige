using ReProServices.Application.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.CustonerTaxLogin
{
    public class CustomerTaxLoginDetails
    {
        public List<CustomerDto> customers { get; set; }
        public bool IsOptedOut { get; set; }
        public string UnitNo { get; set; }
        public DateTime AsOfDate { get; set; }
    }
}
