using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReProServices.Application.Customers;
using ReProServices.Application.Customers.Commands.CreateCustomer;
using ReProServices.Application.Customers.Commands.DeleteCustomer;
using ReProServices.Application.Customers.Commands.ImportCustomers;
using ReProServices.Application.Customers.Commands.UpdateCustomer;
using ReProServices.Application.Customers.Queries;
using ReProServices.Domain;
using ReProServices.Domain.Entities;
using WeihanLi.Npoi;
using ReProServices.Infrastructure.Smtp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    [Authorize]
    public class CustomerController : ApiController
    {
        private IConfiguration _configuration;
        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [Authorize(Roles = "Client_View")]
        [HttpGet]
        public async Task<CustomerVM> Get([FromQuery] CustomerDetailsFilter customerDetailsFilter)
        {
            return await Mediator.Send(new GetCustomersQuery() { Filter = customerDetailsFilter });
        }
        [Authorize(Roles = "Client_View")]
        [HttpGet("getCustomerCount")]
        public async Task<CustomerCountDto> GetCustomerCount()
        {
            return await Mediator.Send(new GetCustomerCountQuery() { });
        }
        [Authorize(Roles = "Client_View")]
        [HttpGet("getExcel")]
        public async Task<FileResult> GetExcel([FromQuery] CustomerDetailsFilter customerDetailsFilter)
        {
            var result = await Mediator.Send(new GetCustomersQuery() { Filter = customerDetailsFilter });
            var resultSet = result.customersView;

            var settings = FluentSettings.For<ViewCustomerPropertyBasic>();
            settings.HasAuthor("REpro Services");

            settings.Property(_ => _.CustomerName)
                .HasColumnTitle("Customer Name")
                .HasColumnWidth(50)
                .HasColumnIndex(0);

            settings.Property(x => x.PAN)
                .HasColumnWidth(16)
                .HasColumnIndex(1);

            settings.Property(x => x.PropertyPremises)
                .HasColumnTitle("Property Premises")
                .HasColumnWidth(30)
                .HasColumnIndex(2);

            settings.Property(x => x.UnitNo)
                .HasColumnTitle("Unit No")
                .HasColumnWidth(18)
                .HasColumnIndex(3);

            settings.Property(x => x.TotalUnitCost)
                .HasColumnTitle("Unit Cost")
                .HasColumnWidth(18)
                .HasColumnIndex(4);

            settings.Property(x => x.DateOfAgreement)
                .HasColumnTitle("Date of Agreement")
                .HasColumnFormatter("dd-MMM-yyy")
                .HasColumnWidth(18)
                .HasColumnIndex(5);

            settings.Property(x => x.DateOfSubmission)
                .HasColumnTitle("Date of Submission")
                .HasColumnFormatter("dd-MMM-yyy")
                .HasColumnWidth(18)
                .HasColumnIndex(6);

            settings.Property(x => x.Remarks)
                .HasColumnTitle("Remarks")
                .HasColumnWidth(60)
                .HasColumnIndex(7);

            settings.Property(x => x.TracesPassword)
              .HasColumnTitle("Traces Password")
              .HasColumnWidth(60)
              .HasColumnIndex(8);

            settings.Property(x => x.CustomerAlias)
             .HasColumnTitle("Alias")
             .HasColumnWidth(60)
             .HasColumnIndex(9);

            settings.Property(x => x.IsPanVerified)
                .HasColumnTitle("Is Pan Verified")
                .HasColumnWidth(60)
                .HasColumnIndex(10);

            settings.Property(x => x.StatusTypeID)
               .HasColumnTitle("Stamp Duty")
               .HasColumnWidth(60)
               .HasColumnIndex(11);

            settings.Property(_ => _.OwnershipID).Ignored();
            settings.Property(_ => _.CustomerID).Ignored();
            settings.Property(_ => _.PropertyID).Ignored();
            settings.Property(_ => _.CustomerPropertyID).Ignored();
            settings.Property(_ => _.OwnershipID).Ignored();
            settings.Property(_ => _.PaymentMethodId).Ignored();
            settings.Property(_ => _.StatusTypeID).Ignored();

            var ms = resultSet.ToExcelBytes();

            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CustomerDetails.xls");

        }
        [Authorize(Roles = "Client_View")]
        [HttpGet("{id}")]
        public async Task<CustomerVM> GetById(Guid id)
        {
            return await Mediator.Send(new GetCustomerByIDQuery { OwnershipId = id });
        }

        [HttpGet("PAN/{pan}")]
        public async Task<CustomerDto> GetByPAN(string pan)
        {
            return await Mediator.Send(new GetCustomerByPANQuery { PAN = pan });
        }
        [Authorize(Roles = "Client_Create")]
        [HttpPost]
        public async Task<ActionResult<CustomerVM>> Create(CreateCustomerCommand command)
        {
            var result = await Mediator.Send(command);
            return result;
        }
        [Authorize(Roles = "Client_Edit")]
        [HttpPut()]
        public async Task<ActionResult<CustomerVM>> Update(UpdateCustomerCommand command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        [HttpDelete("{id}/{ownershipid}")]
        public async Task<Unit> Delete(int id, Guid ownershipId)
        {
            return await Mediator.Send(new DeleteCustomerCommand { CustomerID = id, OwnershipID = ownershipId });
        }

        [HttpPost("uploadFile"), DisableRequestSizeLimit]
        public async Task<bool> UploadCustomerProperty()
        {
            Stopwatch s2 = new Stopwatch();
            s2.Start();
            var files = Request.Form.Files;

            if (files.Any(f => f.Length == 0))
            {
                throw new DomainException("One of the files is empty or corrupt");
            }

            var file = Request.Form.Files[0];
            var ms = new MemoryStream();

            file.CopyTo(ms);
            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(ms))
            {
                var dataTable = new DataTable();
                var filetype = excelReader.GetType().Name;
                if (filetype == "ExcelOpenXmlReader")
                {
                    using (IExcelDataReader reader =
                        ExcelReaderFactory.CreateOpenXmlReader(ms, new ExcelReaderConfiguration()))
                    {
                        dataTable = reader.AsDataSet().Tables[0];
                        DataRow row = dataTable.Rows[0];
                        dataTable.Rows.Remove(row); //removing the headings
                    }
                }
               
                if (filetype == "ExcelBinaryReader")
                {
                    using (IExcelDataReader reader =
                        ExcelReaderFactory.CreateBinaryReader(ms, new ExcelReaderConfiguration()))
                    {
                        dataTable = reader.AsDataSet().Tables[0];
                        DataRow row = dataTable.Rows[0];
                        dataTable.Rows.Remove(row); //removing the headings
                    }
                }

                Console.WriteLine("Records Count = " + dataTable.Rows.Count);

                string errorPan = "";
                Regex regex = new Regex("([A-Z]){5}([0-9]){4}([A-Z]){1}$");

                foreach (DataRow row in dataTable.Rows)
                {
                    var custinerInx = new int[] { 4, 9, 14 };
                    foreach (var pos in custinerInx) {
                        var pan = row[pos].ToString().ToUpper();
                        if (string.IsNullOrEmpty(pan))
                            continue;
                        if (!regex.IsMatch(pan.Trim()))
                        {
                            errorPan += pan + " , ";
                        }
                    }
                   
                }

                if (errorPan != "")
                {
                    throw new DomainException("Invalid PAN cards : " + errorPan);
                }


                await Mediator.Send(new ImportCustomersCommand { dataTable= dataTable });

                return true;              
            }

        }

       
        [HttpGet("welcomeMail/{email}/{project}/{unitNo}")]
        public async Task<bool> SendWelcomeMail(string email,string project,string unitNo)
        {
            var filePath = @Directory.GetCurrentDirectory() + "\\Resources\\logo.png";

            Bitmap b = new Bitmap(filePath);
            MemoryStream ms = new MemoryStream();
            b.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            var logoResource = new LinkedResource(ms, "image/png") { ContentId = "added-image-id" };
            var subject = "TDS compliance under section 194IA for your Unit No." + unitNo + " in project " + project;

            var emilaModel = new EmailModel()
            {
               // To="karthi@leansys.in",
               To = email,
                Subject = subject,
                //Message = @"<html><body> <p>Dear Sir/Madam, </p><p>Greetings from REpro Services!!</p> <p>We wish to inform you that we have been appointed by Prestige Group to manage your TDS compliance U/s. 194 (IA) for your subject property. We are a team of professionals who have expertise in all tax compliances. </p><br> " +
                //        " <p>We request you to let us know if you <b> DO NOT WANT </b> us to manage this TDS compliance on your behalf. </p><br>" +
                //         "<p>If we do not receive any communication in response to this e-Mail, we will proceed with managing your TDS compliance for your unit in the subject project. We will do this compliance for each of your payments to Prestige till you take possession of this property. </p><br>" +
                //         " <p>If you are already registered in Traces as Tax payer, request you to share your traces Password to update our records for downloading Form 16B.</p><br>" +
                //         "<br> <img height='90' width='170'  src=cid:added-image-id><p>Thanks and Regards,<br>REpro Team</p> </body></html> ",
               // Message = @"<html><body> <p>Dear Sir/Madam, </p><p>Greetings from REpro Services!!</p> <p>We wish to inform you that we have been appointed by Prestige Group to manage your TDS compliance U/s. 194 (IA) for your subject property. We are a team of professionals who have expertise in all tax compliances. </p><br> " +

               //  " <p>As a buyer you are supposed to deduct 1% as TDS in all your payments to the seller and remit that as TDS under section 194IA. After TDS payment you need to raise a request for download of Form 16B certificate in traces and share the downloaded certificate to seller. This process has to be done for each payment you will make to the seller on or before the end of the subsequent month. </p><br>" +

               //" <p>Since many Prestige customers found this process to be tedious and cumbersome, Prestige has appointed us to assist in TDS compliance for all its customers. As we will be managing your TDS compliance we request you to pay the installment amount to Prestige without TDS deduction, we will collect your TDS amount, remit with your PAN and share the Form 16B certificate to Prestige on your behalf. </p><br>" +

               //  " <p>We request you to let us know if you <b> DO NOT WANT </b> us to manage this TDS compliance on your behalf. Please note that since <b>Prestige has engaged us directly to assist its customers there will no fee payable to us by you. </b> </p><br>" +

               //          "<p>If we do not receive any communication in response to this e-Mail, we will proceed with managing your TDS compliance for your unit in the subject project. We will do this compliance for each of your payments to Prestige till you take possession of this property. </p><br>" +

               //          " <p>If you are already registered in Traces as Tax payer, request you to share your traces Password to update our records for downloading Form 16B.If you do not wish to share the Traces password with us, please let us know we will share the TDS paid challan, you can generate the Form 16B certificate on your own and share it with Prestige.</p><br>" +

               //          "<br> <img height='90' width='170'  src=cid:added-image-id><p>Thanks and Regards,<br>REpro Team</p> </body></html> ",
                IsBodyHtml = true
            };

            if (project.Trim() == "Prestige Beverly Hills")
            {
                emilaModel.Message = @"<html><body> <p>Dear Sir/Madam, </p><p>Greetings from REpro Services!!</p> <p>We wish to inform you that we have been appointed by Prestige Group to manage your TDS compliance U/s. 194 (IA) for your subject property. We are a team of professionals who have expertise in all tax compliances. </p><br> " +

               " <p>As a buyer you are supposed to deduct 1% as TDS in all your payments to the seller and remit that as TDS under section 194IA. After TDS payment you need to raise a request for download of Form 16B certificate in traces and share the downloaded certificate to seller. This process has to be done for each payment you will make to the seller on or before the end of the subsequent month. </p><br>" +

             " <p>Since many Prestige customers found this process to be tedious and cumbersome, Prestige has appointed us to assist in TDS compliance for all its customers. As we will be managing your TDS compliance we request you to pay the installment amount to Prestige without TDS deduction, we will collect your TDS amount, remit with your PAN and share the Form 16B certificate to Prestige on your behalf. </p><br>" +

               " <p>We wish you avail our service, however if you  <b> DO NOT WANT </b> us to manage this TDS compliance on your behalf please let us know by reverting to this email. Please note that since <b>Prestige has engaged us directly to assist its customers there will no fee payable to us by you. </b> </p><br>" +

                       "<p>Kindly note, if we do not receive any communication in response to this e-Mail within 7 working days we would be assuming that you would like to use our services and we will proceed with managing your TDS compliance for your unit in the subject project. We will do this compliance for each of your payments to Prestige till you take possession of this property. </p><br>" +

                       " <p>If you are already registered in Traces as Tax payer, request you to share your traces Password to update our records for downloading Form 16B. If you do not wish to share the Traces password with us, please let us know we will share the TDS paid challan, you can generate the Form 16B certificate on your own and share it with Prestige. If you have not registered we will register on your behalf and do the needful.</p><br>" +
                      
                       " <p>We would prefer email as our choice of communication, you may write to tdscompliance@reproservices.in, but If you still have any queries and you would like to get it clarified, you can reach out to Prestige Accounts: Mr. Naveen Kumar B (Mobile number: 9121221120).</p><br>" +

                       "<br> <img height='90' width='170'  src=cid:added-image-id><p>Thanks and Regards,<br>REpro Team</p> </body></html> ";
            }
            else
            {
                emilaModel.Message = @"<html><body> <p>Dear Sir/Madam, </p><p>Greetings from REpro Services!!</p> <p>We wish to inform you that we have been appointed by Prestige Group to manage your TDS compliance U/s. 194 (IA) for your subject property. We are a team of professionals who have expertise in all tax compliances. </p><br> " +

              " <p>As a buyer you are supposed to deduct 1% as TDS in all your payments to the seller and remit that as TDS under section 194IA. After TDS payment you need to raise a request for download of Form 16B certificate in traces and share the downloaded certificate to seller. This process has to be done for each payment you will make to the seller on or before the end of the subsequent month. </p><br>" +

            " <p>Since many Prestige customers found this process to be tedious and cumbersome, Prestige has appointed us to assist in TDS compliance for all its customers. As we will be managing your TDS compliance we request you to pay the installment amount to Prestige without TDS deduction, we will collect your TDS amount, remit with your PAN and share the Form 16B certificate to Prestige on your behalf. </p><br>" +

              " <p>We wish you avail our service, however if you  <b> DO NOT WANT </b> us to manage this TDS compliance on your behalf please let us know by reverting to this email. Please note that since <b>Prestige has engaged us directly to assist its customers there will no fee payable to us by you. </b> </p><br>" +

                      "<p>Kindly note, if we do not receive any communication in response to this e-Mail within 7 working days we would be assuming that you would like to use our services and we will proceed with managing your TDS compliance for your unit in the subject project. We will do this compliance for each of your payments to Prestige till you take possession of this property. </p><br>" +

                      " <p>If you are already registered in Traces as Tax payer, request you to share your traces Password to update our records for downloading Form 16B. If you do not wish to share the Traces password with us, please let us know we will share the TDS paid challan, you can generate the Form 16B certificate on your own and share it with Prestige. If you have not registered we will register on your behalf and do the needful.</p><br>" +

                      " <p>For any clarifications you may write to tdscompliance@reproservices.in or contact Prestige CRM team to know about our service.</p><br>" +

                      "<br> <img height='90' width='170'  src=cid:added-image-id><p>Thanks and Regards,<br>REpro Team</p> </body></html> ";
            }

            EmailHelper emailHelper = new EmailHelper(_configuration);
            var isSent = emailHelper.SendEmail(emilaModel, logoResource);
            return isSent;
        }
    }
}