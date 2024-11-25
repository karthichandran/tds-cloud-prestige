using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReProServices.Domain.Entities.ClientPortal
{
    public class InfoContent
    {
        [Key]
        public int NotifyId { get; set; }
        public string ProfileTxt { get; set; }
        public string PaymentToSeller { get; set; }
        public string Form16B { get; set; }
        public string TdsCompliance { get; set; }
        public string LoginPopUp { get; set; }
        public string Faq { get; set; }
        public bool? PossessionUnit { get; set; }
    }
}
