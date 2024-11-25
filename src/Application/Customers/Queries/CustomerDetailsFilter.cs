namespace ReProServices.Application.Customers.Queries
{
    public class CustomerDetailsFilter
    {
        public string CustomerName { get; set; }
        public string PAN { get; set; }
        public string UnitNo { get; set; } 
        public int StatusTypeId { get; set; } = 0;
        public int PropertyId { get; set; } = 0;
        public string Remarks { get; set; }
        public string Premises { get; set; }
        
    }
}
