using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.CustonerTaxLogin
{
    public class CustomerTaxPasswordDto
    {
        public int CustomerTaxLoginId { get; set; }
        public string UnitNo { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string TaxPassword { get; set; }
        public string Pan { get; set; }
        public bool IsOptOut { get; set; }
        public bool IsSelected { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.CustomerTaxLogin, CustomerTaxPasswordDto>();
        }
    }
}
