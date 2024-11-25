using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReProServices.Domain.Entities
{
   public class ProspectProperty
    {
        [Key]
        public int ProspectPropertyID { get; set; }
        public int PropertyID { get; set; }
        public string UnitNo { get; set;}
        public DateTime DeclarationDate { get; set; }
        public Guid? OwnershipID { get; set; }
    }
}
