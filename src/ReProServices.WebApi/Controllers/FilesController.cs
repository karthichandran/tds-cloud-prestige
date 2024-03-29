﻿using System;
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
using ReProServices.Infrastructure.GoogleDrive;
using ReProServices.Infrastructure.MegaDrive;

namespace WebApi.Controllers
{
    //[Authorize]
    public class FilesController : ApiController
    {
        private DriverService driverSrv;
        private MegaDriveService megaSrv;
        public FilesController() {
            driverSrv = new DriverService();
            megaSrv = new MegaDriveService();
        }

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
                    //custPropFile.FileBlob = ms.ToArray();
                    custPropFile.FileBlob = new byte[1];
                    custPropFile.FileType = file.ContentType;
                    custPropFile.FileCategoryId = categoryId;

                    //var gdid=await driverSrv.AddFile(custPropFile.FileName, custPropFile.FileType, ms);
                    //custPropFile.GDfileID = gdid;

                    var status = await megaSrv.UploadFile(ms, custPropFile.FileName);
                    if (status)
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
                    //custPropFile.FileBlob = ms.ToArray();
                    custPropFile.FileBlob = new byte[1];
                    custPropFile.FileType = file.ContentType;
                    custPropFile.FileCategoryId = categoryId;

                    //var gdid = await driverSrv.AddFile(custPropFile.FileName, custPropFile.FileType, ms);
                    //custPropFile.GDfileID = gdid;
                    var status = await megaSrv.UploadFile(ms, custPropFile.FileName);
                    if (status)
                    custPropFiles.Add(custPropFile);                   

                }


                bool result = await Mediator.Send(new UploadCustomerPropertyFileCommand { CustomerPropertyFiles = custPropFiles });
                return Ok(result);
                // return Ok();
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
                //custPropFile.FileBlob = ms.ToArray();
                custPropFile.FileBlob = new byte[1];
                custPropFile.FileType = files[0].ContentType;
                custPropFile.FileCategoryId = 5;

                //var gdid = await driverSrv.AddFile(custPropFile.FileName, custPropFile.FileType, ms);
                //custPropFile.GDfileID = gdid;
               var status= await megaSrv.UploadFile(ms, custPropFile.FileName);
                if(!status)
                    throw new DomainException("The files is empty or  corrupt");

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


            // Dont want files data
           // var files = await Mediator.Send(new GetCustomerPropertyFilesListQuery { OwnershipID = ownershipID, GetFilesToo = false });
            //foreach (var file in files)
            //{
            //    if (file.GDfileID != null)
            //    {
            //        var ms = driverSrv.GetFile(file.GDfileID);
            //        file.FileBlob = ms.ToArray();
            //    }
            //}
           // return files;


            //======== migrate files from sql to drive
            //int start = 371, end = 372;
            //do
            //{

            //    var files = await Mediator.Send(new GetCustomerPropertyFilesListQuery { OwnershipID = ownershipID, GetFilesToo = false, start = start, end = end });
            //    //if (files.Count() == 0)
            //    //    break;
            //    foreach (var file in files)
            //    {
            //        if (file.GDfileID == null) { 
            //            var gdid = await driverSrv.AddFile(file.FileName, file.FileType, new MemoryStream(file.FileBlob));

            //        await Mediator.Send(new UpdateGoogelDriveFileIdCommand { blobId = file.BlobID, gdId = gdid });}
            //    }

            //    start = end;
            //    end = end + 1;
            //} while (true);

            // return new List< CustomerPropertyFileDto>();
        }

        [HttpGet("blobId/{blobID}")]
        public async Task<FileResult> GetFileByBlobID(int blobID)
        {
            var binaries = await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID = blobID});
            //return File(binaries.FileBlob, binaries.FileType, binaries.FileName);
            if (binaries == null)
                return null;

            if (binaries.GDfileID == null)
            {
                if (binaries.FileBlob.Length > 10)
                    return File(binaries.FileBlob, binaries.FileType, binaries.FileName);

                var ms = await megaSrv.DownloadFile(binaries.FileName);
                return File(ms.ToArray(), binaries.FileType, binaries.FileName);
            }           
            else
            {
                var ms = driverSrv.GetFile(binaries.GDfileID);
                return File(ms.ToArray(), binaries.FileType, binaries.FileName);
            }
        }

        [HttpGet("fileinfo/{blobID}")]
        public async Task<CustomerPropertyFileDto> GetFileInfoByBlobID(int blobID)
        {
           var file=  await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID = blobID });
            if (file == null)
                return null;

            if (file.GDfileID == null)
            {
                if (file.FileBlob.Length > 10)
                    return file;

                var ms = await megaSrv.DownloadFile(file.FileName);
                file.FileBlob = ms.ToArray();
                return file;
            }
            else
            {
                var ms = driverSrv.GetFile(file.GDfileID);
                file.FileBlob = ms.ToArray();
                return file;
            }
        }

        [HttpGet("fileDetails/panID/{panID}")]
        public async Task<CustomerPropertyFileDto> GetFileDetailsByPanID(string panID)
        {
            var file= await Mediator.Send(new GetCustomerPropertyFileByPanIdQuery { PanID = panID });
            if (file == null)
                return null;

            if (file.GDfileID == null) {
                if (file.FileBlob.Length > 10)
                    return file;

                var ms =await megaSrv.DownloadFile(file.FileName);
                file.FileBlob = ms.ToArray();
                return file;
            }               
            else
            {
                var ms = driverSrv.GetFile(file.GDfileID);
                file.FileBlob = ms.ToArray();
                return file;
            }
        }
                                   
        [HttpDelete("blobId/{blobID}")]
        public async Task<IActionResult> DeleteFileByBlobID(int blobID)
        {
            // Delete a file from ggogel drive 
            var binaries = await Mediator.Send(new GetCustomerPropertyFileByBlobIdQuery { FileID = blobID });
            if (binaries != null && binaries.GDfileID!=null) {
                driverSrv.DeleteFile(binaries.GDfileID);
            }
            else if (binaries != null ){
               await megaSrv.DeleteFile(binaries.FileName);
            }

            bool result = await Mediator.Send(new DeletePropertyFileCommand { BlobID = blobID });
            return Ok(result);
        }

    }
}