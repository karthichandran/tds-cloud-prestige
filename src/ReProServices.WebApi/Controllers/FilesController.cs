using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReProServices.Application.CustomerPropertyFiles;
using ReProServices.Application.CustomerPropertyFiles.Commands.DeleteCustomerPropertyFile;
using ReProServices.Application.CustomerPropertyFiles.Commands.UploadCustomerProeprtyFile;
using ReProServices.Application.CustomerPropertyFiles.Queries;
using ReProServices.Domain;

namespace WebApi.Controllers
{
    //[Authorize]
    public class FilesController : ApiController
    {
        [HttpPost("Guid/{guid}/{categoryId}"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(Guid guid, int categoryId )
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Any(f => f.Length == 0))
                {
                    throw new DomainException("One of the files is empty or  corrupt");
                }

                IList<CustomerPropertyFileDto> custPropFiles = new List<CustomerPropertyFileDto>();
                foreach (var file in files)
                {
                    CustomerPropertyFileDto custPropFile = new CustomerPropertyFileDto
                    {
                        FileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'),
                        OwnershipID = guid
                    };

                    var ms = new MemoryStream();
                    await file.OpenReadStream().CopyToAsync(ms);
                    custPropFile.FileBlob = ms.ToArray();
                    custPropFile.FileType = file.ContentType;
                    custPropFile.FileCategoryId = categoryId;
                    custPropFiles.Add(custPropFile);

                }
                bool result = await Mediator.Send(new UploadCustomerPropertyFileCommand { CustomerPropertyFiles = custPropFiles });
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost("PanId/{panId}/{categoryId}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPan(string panId, int categoryId)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Any(f => f.Length == 0))
                {
                    throw new DomainException("One of the files is empty or  corrupt");
                }

                IList<CustomerPropertyFileDto> custPropFiles = new List<CustomerPropertyFileDto>();
                foreach (var file in files)
                {
                    CustomerPropertyFileDto custPropFile = new CustomerPropertyFileDto
                    {
                        FileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'),
                        PanID = panId
                    };

                    var ms = new MemoryStream();
                    await file.OpenReadStream().CopyToAsync(ms);
                    custPropFile.FileBlob = ms.ToArray();
                    custPropFile.FileType = file.ContentType;
                    custPropFile.FileCategoryId = categoryId;
                    custPropFiles.Add(custPropFile);

                }
                bool result = await Mediator.Send(new UploadCustomerPropertyFileCommand { CustomerPropertyFiles = custPropFiles });
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost("PanId/{panId}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPanFile(string panId)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Any(f => f.Length == 0))
                {
                    throw new DomainException("One of the files is empty or  corrupt");
                }

                CustomerPropertyFileDto custPropFile = new CustomerPropertyFileDto
                {
                    FileName = ContentDispositionHeaderValue.Parse(files[0].ContentDisposition).FileName.Trim('"'),
                    PanID = panId
                };

                var ms = new MemoryStream();
                await files[0].OpenReadStream().CopyToAsync(ms);
                custPropFile.FileBlob = ms.ToArray();
                custPropFile.FileType = files[0].ContentType;
                custPropFile.FileCategoryId = 5;

                int result = await Mediator.Send(new UploadPanFileCommand { CustomerPropertyFile = custPropFile });
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("fileslist/{ownershipID}")]
        public async Task<IList<CustomerPropertyFileDto>> GetById(Guid ownershipID)
        {
            return await Mediator.Send(new GetCustomerPropertyFilesListQuery { OwnershipID = ownershipID, GetFilesToo = false }); 
        }

        [HttpGet("blobId/{blobID}")]
        public async Task<FileResult> GetFileByBlobID(int blobID)
        {
            var binaries = await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID = blobID});
            return File(binaries.FileBlob, binaries.FileType, binaries.FileName); 
        }

        [HttpGet("fileinfo/{blobID}")]
        public async Task<CustomerPropertyFileDto> GetFileInfoByBlobID(int blobID)
        {
           return await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID = blobID });
        }

        [HttpGet("fileDetails/panID/{panID}")]
        public async Task<CustomerPropertyFileDto> GetFileDetailsByPanID(string panID)
        {
            return await Mediator.Send(new GetCustomerPropertyFileByPanIdQuery { PanID = panID });
        }
                                   
        [HttpDelete("blobId/{blobID}")]
        public async Task<IActionResult> DeleteFileByBlobID(int blobID)
        {
            bool result = await Mediator.Send(new DeletePropertyFileCommand { BlobID = blobID });
            return Ok(result);
        }

    }
}