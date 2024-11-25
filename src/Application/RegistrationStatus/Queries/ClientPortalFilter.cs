using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.RegistrationStatus.Queries
{
   public class ClientPortalFilter
    {
        public int ProjectId { get; set; }
        public int CustomerId { get; set; }
        public string UnitNo { get; set; }
    }
}
