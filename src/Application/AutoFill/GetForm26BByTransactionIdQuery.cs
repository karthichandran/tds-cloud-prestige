using System;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReProServices.Domain.Extensions;

namespace ReProServices.Application.AutoFill
{
    public class GetForm26BByTransactionIdQuery : IRequest<AutoFillDto>
    {
        public int ClientPaymentTransactionID { get; set; }
        public class GetForm26BByTransactionIdQueryHandler :
                              IRequestHandler<GetForm26BByTransactionIdQuery, AutoFillDto>
        {

            private readonly IApplicationDbContext _context;

            public GetForm26BByTransactionIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<AutoFillDto> Handle(GetForm26BByTransactionIdQuery request, CancellationToken cancellationToken)
            {
                                
                var vm = await (from pay in _context.ClientPayment
                                join cpt in _context.ClientPaymentTransaction on new { pay.InstallmentID, pay.ClientPaymentID } equals new { cpt.InstallmentID, cpt.ClientPaymentID }
                                join cp in _context.ViewCustomerPropertyExpanded on new { cpt.OwnershipID, cpt.CustomerID } equals new { cp.OwnershipID, cp.CustomerID }
                                join sp in _context.ViewSellerPropertyExpanded on cp.PropertyID equals sp.PropertyID
                                where cpt.ClientPaymentTransactionID == request.ClientPaymentTransactionID
                                select new AutoFillDto
                                {
                                    tab1 = new Tab1
                                    {
                                        StatusOfPayee = sp.SellerIsResident,
                                        PanOfPayer = cp.CustomerPAN,
                                        PanOfTransferor = sp.SellerPAN,
                                        TaxApplicable =  cp.CustomerPAN[3] == 'C' ? "0020" : "0021"
                                    },

                                    tab2 = new Tab2()
                                    {
                                        //transferee
                                        AddressPremisesOfTransferee = cp.CustomerAddressPremises,
                                        AdressLine1OfTransferee = cp.CustomerAddressLine1.Trim(),
                                        AddressLine2OfTransferee = cp.CustomerAddressLine2.Trim(),
                                        CityOfTransferee = cp.CustomerCity.Trim(),
                                        StateOfTransferee = cp.CustomerFrom26BState.Trim(),
                                        PinCodeOfTransferee = cp.CustomerPinCode.Trim(),
                                        EmailOfOfTransferee = cp.CustomerEmailID.Trim(),
                                        MobileOfOfTransferee = cp.CustomerMobileNo,
                                        IsCoTransferee = cp.IsUnitShared,

                                        //transferor
                                        AddressPremisesOfTransferor = sp.SellerPremises,
                                        AddressLine1OfTransferor = sp.SellerAddressLine1.Trim(),
                                        AddressLine2OfTransferor = sp.SellerAddressLine2.Trim(),
                                        CityOfTransferor = sp.SellerCity.Trim(),
                                        PinCodeOfTransferor = sp.SellerPinCode.Trim(),
                                        StateOfTransferor = sp.Seller26BState.Trim(),
                                        MobileOfOfTransferor = sp.SellerMobileNo,
                                        EmailOfOfTransferor = sp.SellerEmailID.Trim(),
                                        IsCoTransferor = sp.IsSharedSeller,
                                    },

                                    tab3 = new Tab3()
                                    {
                                        AddressPremisesOfProperty = cp.PropertyPremises.Trim(),
                                        AddressLine1OfProperty = cp.UnitNo.ToString(),
                                        //AddressLine1OfProperty = cp.PropertyAddressLine1.Trim(),
                                        AddressLine2OfProperty = cp.PropertyAddressLine2.Trim(),
                                        CityOfProperty = cp.PropertyCity.Trim(),
                                        StateOfProperty = cp.PropertyForm26BState.Trim(),
                                        PinCodeOfProperty = cp.PropertyPinCode.Trim(),
                                        DateOfAgreement = cp.DateOfAgreement.Value.GenerateDatePart(),
                                        LateFee = cpt.LateFee,
                                        TotalAmount = (int)cp.TotalUnitCost.Value,
                                        AmountPaid = (int)cpt.GrossShareAmount,
                                        Interest = cpt.TdsInterest,
                                        AmountPaidParts = ((int)cpt.GrossShareAmount).GeneratePlaceValues(),
                                        BasicTax = (int)Math.Ceiling(cpt.Tds),
                                        PaymentType = cp.PaymentMethodID,
                                        TypeOfProperty = (sp.PropertyType == 1) ? "Land" : "Building",
                                        OwnershipId = pay.OwnershipID,
                                        InstallmentId = cpt.InstallmentID,
                                        PropertyID = sp.PropertyID,
                                        RevisedDateOfPayment=pay.RevisedDateOfPayment.GenerateDatePart(),
                                        DateOfDeduction=pay.DateOfDeduction.GenerateDatePart()
                                    },

                                    tab4 = new Tab4()
                                    {
                                        //DateOfTaxDeduction = pay.DateOfDeduction.GenerateDatePart(),
                                       // DateOfPayment = pay.RevisedDateOfPayment.GenerateDatePart(),
                                        ModeOfPayment = "modeBankSelection"
                                    }
                                })
                               .FirstOrDefaultAsync(cancellationToken);
                return vm;
            }

        }
    }
}
