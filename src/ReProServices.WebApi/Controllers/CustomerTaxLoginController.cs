using Microsoft.AspNetCore.Mvc;
using ReProServices.Application.CustonerTaxLogin;
using ReProServices.Application.CustonerTaxLogin.Command;
using ReProServices.Application.CustonerTaxLogin.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    public class CustomerTaxLoginController : ApiController
    {
        [HttpGet("{propertyid}")]
        public async Task<List<UnitNumberDto>> Get(int propertyid)
        {
            return await Mediator.Send(new GetUnitsByPropertyQuery() {propertyId= propertyid });
        }

        [HttpGet("customersByUnit/{propertyId}/{unitNo}")]
        public async Task<CustomerTaxLoginDetails> GetCustomers(int propertyId, string unitNo)
        {
            return await Mediator.Send(new GetCustomersByUnitNoQuery() { PropertyId=propertyId, UnitNo = unitNo });
        }

        [HttpPost]
        public async Task<bool> UpdateCustomers(CustomerTaxLoginDetails customers)
        {
            return await Mediator.Send(new CreateCustomerTaxLoginCommand() { customers=customers });
        }

        [HttpGet("postedcustomers/{unitNo}")]
        public async Task<List<CustomerTaxPasswordDto>> GetPostedCustomers(string unitNo)
        {
            return await Mediator.Send(new GetCustomerTaxPasswordQuery() { unitNo = unitNo });
        }

        [HttpPost("updateitpwd")]
        public async Task<bool> UpdateCustomersItPassword(List<CustomerTaxPasswordDto> customers)
        {
            return await Mediator.Send(new UpdateCustomerTaxPasswordCommand() { customers = customers });
        }
    }
}
