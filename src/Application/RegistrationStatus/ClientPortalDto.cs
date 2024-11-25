using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.RegistrationStatus
{
    public class ClientPortalDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string UnitNo { get; set; }
        public DateTime? Registered { get; set; }
        public string Pan { get; set; }
        public int UserId { get; set; }
        public string Pwd { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
