using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReProServices.Domain.Entities.ClientPortal
{
    public class LoginUser
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Email { get; set; }
    }
}
