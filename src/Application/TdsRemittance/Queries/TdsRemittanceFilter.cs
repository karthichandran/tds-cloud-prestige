namespace ReProServices.Application.TdsRemittance.Queries
{
    public class TdsRemittanceFilter
    {
        public string UnitNo { get; set; }
        public string FromUnitNo { get; set; }
        public string ToUnitNo { get; set; }
        public string PropertyPremises { get; set; }
        public int LotNo { get; set; }
        public string CustomerName { get; set; }
        public int? RemittanceStatusID { get; set; }
        public int TdsAmount { get; set; }
    }
}
