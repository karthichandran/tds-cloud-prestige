using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ReProServices.Application.Customers.Queries;
using ReProServices.Application.Prospect.Queries;
using ReProServices.Application.RegistrationStatus;
using ReProServices.Application.RegistrationStatus.Comments;
using ReProServices.Application.RegistrationStatus.Queries;
using WeihanLi.Npoi;
using ReProServices.Application.InfoContent;

namespace WebApi.Controllers
{

    [Authorize]
    public class PortalRegistrationController : ApiController
    {

        public PortalRegistrationController()
        {
            
        }

        [HttpGet("setup")]
        public async Task<ClientPortalSetupDto> GetSetup()
        {
            return await Mediator.Send(new GetClientPortalSetupQuery());
        }

        [HttpGet("customer-unitno/{propertyId}")]
        public async Task<CustomerUnitNoModel> GetCust(int propertyId)
        {
            return await Mediator.Send(new GetCustomerAndUnitNoQuery(){ProjectId = propertyId });
        }

        [HttpGet("user-list")]
        public async Task<List<ClientPortalDto>> GetSetup([FromQuery] ClientPortalFilter filter)
        {
            return await Mediator.Send(new GetClientPortalListQuery(){Filter = filter });
        }

        [HttpGet("entroll-users")]
        public async Task<bool> Get()
        {
            return await Mediator.Send(new CreateUserLoginExistingCustomerCommand());
        }

        [HttpPut("update-pwd")]
        public async Task<bool> UpdateUserPwd(UserLoginModel model)
        {
            return await Mediator.Send(new UpdateUserPasswordCommand{ UserModel = model});
        }


        [HttpGet("infocontent/{status}")]
        public async Task<InfoContentDto> GetInfoContent(int status)
        {
            return await Mediator.Send(new GetInfoContentQuery{PossessionUnit=status==1});
        }

        [HttpPut("save-info")]
        public async Task<bool> UpdateInfoContent(InfoContentDto model)
        {
            return await Mediator.Send(new SaveInfoContentCommand {  info=model });
        }

        [HttpGet("getExcel")]
        public async Task<FileResult> GetExcel([FromQuery] ClientPortalFilter filter)
        {
           
            var resultSet = await Mediator.Send(new GetClientPortalListQuery() { Filter = filter });

            var settings = FluentSettings.For<ClientPortalDto>();
            settings.HasAuthor("REpro Services");

            settings.Property(_ => _.ProjectName)
                .HasColumnTitle("Project Name")
                .HasColumnWidth(50)
                .HasColumnIndex(0);

            settings.Property(_ => _.CustomerName)
                .HasColumnTitle("Customer Name")
                .HasColumnWidth(50)
                .HasColumnIndex(1);

            settings.Property(x => x.UnitNo)
                .HasColumnTitle("Unit No")
                .HasColumnWidth(16)
                .HasColumnIndex(2);

            settings.Property(x => x.Registered)
                .HasColumnTitle("Registered on")
                .HasColumnFormatter("dd-MMM-yyy")
                .HasColumnWidth(18)
                .HasColumnIndex(3);

            settings.Property(x => x.Pan)
                .HasColumnTitle("User ID")
                .HasColumnWidth(18)
                .HasColumnIndex(4);

            settings.Property(x => x.Pwd)
                .HasColumnTitle("Password")
                .HasColumnWidth(18)
                .HasColumnIndex(5);

            settings.Property(x => x.LastUpdated)
                .HasColumnTitle("Last Login Date & Time")
                .HasColumnFormatter("dd-MMM-yyy")
                .HasColumnWidth(18)
                .HasColumnIndex(6);

            settings.Property(_ => _.CustomerId).Ignored();
            settings.Property(_ => _.UserId).Ignored();
            settings.Property(_ => _.ProjectId).Ignored();
          
            var ms = resultSet.ToExcelBytes();

            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RegistrationStatus.xls");

        }
    }
}
