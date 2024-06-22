using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReProServices.Application.ClientPayments.Commands;
using ReProServices.Application.Remittances.Commands.CreateRemittance;
using ReProServices.Application.TransactionLog;

namespace WebApi.Controllers
{
    public class TransactionLogController : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> Create(AddTransactionLogComment command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
