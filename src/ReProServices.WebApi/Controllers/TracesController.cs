using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReProServices.Application.CustomerPropertyFiles;
using ReProServices.Application.CustomerPropertyFiles.Commands.UploadCustomerProeprtyFile;
using ReProServices.Application.CustomerPropertyFiles.Queries;
using ReProServices.Application.Customers.Queries;
using ReProServices.Application.Remittances.Commands.UpdateRemittance;
using ReProServices.Application.Remittances.Queries;
using ReProServices.Application.TdsRemittance;
using ReProServices.Application.TdsRemittance.Queries;
using ReProServices.Application.Traces;
using ReProServices.Application.Traces.Command;
using ReProServices.Domain;
using ReProServices.Infrastructure.Smtp;

namespace WebApi.Controllers
{
    //[Authorize]
    public class TracesController : ApiController
    {
        private IConfiguration _configuration;
       
        public TracesController(IConfiguration configuration) {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IList<TdsRemittanceDto>> Get([FromQuery]TdsRemittanceFilter tdsRemittanceFilter)
        {
            return await Mediator.Send(new GetTracesListQuery() { Filter = tdsRemittanceFilter });
        }


        [HttpGet("{transactionID}")]
        public async Task<TdsRemittanceDto> GetById(int transactionID)
        {
            return await Mediator.Send(new GetTracesByTransactionIDQuery() { ClientPaymentTransactionID = transactionID });
        }

        [HttpPost("{remittanceID}/{categoryId}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadRemittanceFile(int remittanceID, int categoryId)
        {
            try
            {
                var file = Request.Form.Files.FirstOrDefault();
                if (file.Length == 0)
                {
                    throw new DomainException("One of the files is empty or  corrupt");
                }

                CustomerPropertyFileDto custPropFile = new CustomerPropertyFileDto
                {
                    FileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'),
                };

                var ms = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(ms);
                custPropFile.FileBlob = ms.ToArray();
                custPropFile.FileType = file.ContentType;
                custPropFile.FileCategoryId = categoryId;

                var result = await Mediator.Send(new UploadRemittanceFileCommand { CustomerPropertyFile = custPropFile,RemittanceID=remittanceID });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTracesCommand() { RemittanceID = id });
            return NoContent();
        }

        [HttpGet("sendmail/{clientPayTransId}")]
        public async Task<ActionResult> SendMail(int clientPayTransId)
        {
             var remittanceModel=await Mediator.Send(new GetRemittancesQuery() { ClientPaymentTransactionID = clientPayTransId });
             var customerModel=await Mediator.Send(new GetCustomerByPANQuery { PAN = remittanceModel.CustomerPAN });

            var filePath = @Directory.GetCurrentDirectory() + "\\Resources\\logo.png";
            Bitmap b = new Bitmap(filePath);
            MemoryStream ms = new MemoryStream();
            b.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            var logoResource = new LinkedResource(ms, "image/png") { ContentId = "added-image-id" };
            
            //var imageResource = new LinkedResource(filePath) { ContentId = "added-image-id" };

            //string imageData = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

            // var binaries2 = await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID = 350 });
            //var binaries2 = await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID = 593 });
            //string imageData = "data:image/png;base64," + Convert.ToBase64String(new MemoryStream(binaries2.FileBlob).ToArray()) ;
            // var imageResource = new LinkedResource(Convert.ToBase64String(binaries2.FileBlob), "image/png") { ContentId = "added-image-id" };

            var form16b = await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID =Convert.ToInt32( remittanceModel.Form16BlobID) });
             var challan = await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID = Convert.ToInt32(remittanceModel.ChallanBlobID) });

            var subject = "Form 16B - " + remittanceModel.Premises + " - " + remittanceModel.UnitNo;
            var emilaModel = new EmailModel()
            {
                To=customerModel.EmailID,
               // To = "karthi@leansys.in",
                Subject = subject,
                Message = @"<html><body> <p>Dear Sir/Madam, </p> <p>Greetings from REpro Services!!</p>  <p>Please find attached challan and Form 16B for the payments made by you to Prestige.<b> THIS IS FOR YOUR INFORMATION ONLY </b>, the same has been shared with Seller from our end for due credit in your statement of account.</p>"+
					 	  "<p>You will receive challan and Form 16B for the payments made during the month by last week of the subsequent month.</p>  "+
                          " <img height='90' width='170'  src=cid:added-image-id><p>Thanks and Regards,<br>R Ganesh / Sriram B Iyer <br>+91 9620508968 / +91 9663751471</p>  </body></html> ",
                IsBodyHtml = true
            };
            List<FileAttachment> fileList = new List<FileAttachment>();
            if(form16b!=null)
                fileList.Add(new FileAttachment() { MemoryStream = form16b.FileBlob, FileName = form16b.FileName, FileType = form16b.FileType });
            if (challan != null)
                fileList.Add(new FileAttachment() { MemoryStream = challan.FileBlob, FileName = challan.FileName, FileType = challan.FileType });
            emilaModel.attachments = fileList;

            EmailHelper emailHelper = new EmailHelper(_configuration);
            emailHelper.SendEmail(emilaModel, logoResource);
            await Mediator.Send(new UpdateEmailSentCommand() { RemittanceID = remittanceModel.RemittanceID });
            return NoContent();
        }
    }
}