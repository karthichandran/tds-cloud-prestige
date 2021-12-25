using System;
using ReProServices.Domain.Common;

namespace ReProServices.Application.AutoFill
{
    public class AutoFillDto
    {
        public AutoFillDto()
        {
                tab1 = new Tab1(); 
                tab2 = new Tab2(); 
                tab3 = new Tab3(); 
                tab4 = new Tab4(); 
        }

        public Tab1 tab1 { get; set; }
        public Tab2 tab2 { get; set; }
        public Tab3 tab3 { get; set; }
        public Tab4 tab4 { get; set; }
    }

    public class Tab1
    {
        public string TaxApplicable { get; set; }

        public bool StatusOfPayee { get; set; }


        public string PanOfPayer { get; set; }
        public string PanOfTransferor { get; set; }

    }
    public class Tab2
    {
        //Transferee / buyer
        public string AddressPremisesOfTransferee { get; set; }
        public string AdressLine1OfTransferee { get; set; }
        public string AddressLine2OfTransferee { get; set; }
        public string CityOfTransferee { get; set; }
        public string StateOfTransferee { get; set; }
        public string PinCodeOfTransferee { get; set; }
        public string EmailOfOfTransferee { get; set; }
        public string MobileOfOfTransferee { get; set; }
        public bool IsCoTransferee { get; set; }



        //transferor/seller
        public string AddressPremisesOfTransferor { get; set; }
        public string AddressLine1OfTransferor { get; set; }
        public string AddressLine2OfTransferor { get; set; }
        public string CityOfTransferor { get; set; }
        public string StateOfTransferor { get; set; }
        public string PinCodeOfTransferor { get; set; }
        public string EmailOfOfTransferor { get; set; }
        public string MobileOfOfTransferor { get; set; }
        public bool IsCoTransferor { get; set; }

    }
    /// <summary>
    /// Property Details
    /// </summary>
    public class Tab3
    {
        //Property Details
        public string TypeOfProperty { get; set; }
        public string AddressPremisesOfProperty { get; set; }
        public string AddressLine1OfProperty { get; set; }
        public string AddressLine2OfProperty { get; set; }
        public string CityOfProperty { get; set; }
        public string StateOfProperty { get; set; }
        public string PinCodeOfProperty { get; set; }
        public DatePart DateOfAgreement { get; set; }
      
        public int TotalAmount { get; set; }
        public int PaymentType { get; set; }
        public DatePart RevisedDateOfPayment { get; set; }
        public DatePart DateOfDeduction { get; set; }
        public PlaceValues AmountPaidParts { get; set; }
        public int AmountPaid { get; set; }
        public Decimal BasicTax { get; set; }
        public Decimal Interest { get; set; }
        public Decimal LateFee { get; set; }


        public Guid OwnershipId { get; set; }
        public Guid InstallmentId { get; set; }
        public int PropertyID { get; set; }
    }

    public class Tab4
    {

        //Payment Info
        public string ModeOfPayment { get; set; }
        public DatePart DateOfPayment { get; set; }
        public DatePart DateOfTaxDeduction { get; set; }
    }


   
}
