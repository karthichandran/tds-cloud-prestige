using System;
using System.Collections.Generic;
using System.Text;

namespace ReProServices.Application.RegistrationStatus
{
   public class ClientPortalSetupDto
    {
        public List< (int,string)> ProjectName{ get; set; }
        public List< (int,string)> CustomerName{ get; set; }
        public List< (string,string)> UnitNo{ get; set; }
    }

   public class CustomerUnitNoModel
   {
       public List<DropdownModel> CustomerName { get; set; }
       public List<DropdownModel> UnitNo { get; set; }
   }


    public class DropdownModel
   { 
       public int Id { get; set; }
       public string Description { get; set; }
    }
}
