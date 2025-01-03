﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ReProServices.Domain.Entities
{
    public class CustomerPropertyFile 
    {
        [Key]
        public int BlobID { get; set; }
        public Guid? OwnershipID { get; set; }
        public string FileName { get; set; }
        public byte[] FileBlob { get; set; }
        public DateTime? UploadTime { get; set; }
        public int? FileLength { get; set; }

        public string FileType { get; set; }

        public string PanID { get; set; }
        public string GDfileID { get; set; }
        public int FileCategoryId { get; set; } = 4;
        public bool? IsFileUploaded { get; set; }
    }
}
