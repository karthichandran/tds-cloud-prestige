using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReProServices.Domain.Entities
{
    [Table("ViewCustomerWithoutProperty")]
   public class ViewCustomerWithoutProperty
    {
   [Key]
   public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string PAN { get; set; }
        public string IsPanVerified { get; set; }
        public string CustomerStatus { get; set; }
        public DateTime? CustomerOptingOutDate { get; set; }
        public string CustomerOptingOutRemarks { get; set; }
        public DateTime? InvalidPanDate { get; set; }
        public string InvalidPanRemarks { get; set; }

    }
}
