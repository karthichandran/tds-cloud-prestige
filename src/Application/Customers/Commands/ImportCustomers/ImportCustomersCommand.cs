using MediatR;
using ReProServices.Application.Common.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using ReProServices.Application.Customers;
using System;
using ReProServices.Domain.Enums;
using System.Linq;
using System.Data;
using ReProServices.Domain.Entities;
using System.Collections.Generic;
using System.Transactions;
using System.Globalization;

namespace ReProServices.Application.Customers.Commands.ImportCustomers
{
    public class ImportCustomersCommand : IRequest<bool>
    {
        public DataTable dataTable { get; set; }
        public class ImportCustomersCommandHandler : IRequestHandler<ImportCustomersCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public ImportCustomersCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<bool> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
            {
                foreach (DataRow row in request.dataTable.Rows)
                {
                    //if (row[4].ToString() != "")
                    //{
                    //    var panExist = _context.Customer.FirstOrDefault(x => x.PAN == row[4].ToString());
                    //    if (panExist != null)
                    //        throw new ApplicationException(" PAN is already exist " + panExist.PAN);
                    //}
                    //if (row[9].ToString() != "")
                    //{
                    //   var panExist = _context.Customer.FirstOrDefault(x => x.PAN == row[9].ToString());
                    //    if (panExist != null)
                    //        throw new ApplicationException(" PAN is already exist " + panExist.PAN);
                    //}
                    //if (row[14].ToString() != "")
                    //{
                    //  var  panExist = _context.Customer.FirstOrDefault(x => x.PAN == row[14].ToString());
                    //    if (panExist != null)
                    //        throw new ApplicationException(" PAN is already exist " + panExist.PAN);
                    //}

                    if(string.IsNullOrEmpty( row[22].ToString()))
                        throw new ApplicationException(" State should not be empty " );

                    var stateObj = _context.StateList.FirstOrDefault(x => x.State.Contains(row[25].ToString()));
                    if (stateObj == null)
                        throw new ApplicationException(" State does not exist " + stateObj);

                    var porperty = _context.Property.FirstOrDefault(x => x.AddressPremises == row[1].ToString());

                    if (porperty == null)
                        throw new ApplicationException("Property does not exist " + porperty);
                }



                //using (TransactionScope transactionscope = new TransactionScope( TransactionScopeOption.RequiresNew, TimeSpan.FromMinutes(10),TransactionScopeAsyncFlowOption.Enabled))
                using (TransactionScope transactionscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    
                    try
                    {
                        foreach (DataRow row in request.dataTable.Rows)
                        {
                           // var existCusPan = _context.Customer.ToList();
                            //var stateObj = _context.StateList.FirstOrDefault(x => x.State.Contains(row[22].ToString()));
                            var stateObj = _context.StateList.FirstOrDefault(x => x.State.Contains(row[25].ToString()));
                            var porperty = _context.Property.FirstOrDefault(x => x.AddressPremises == row[1].ToString());

                            //var custinerInx = new int[] { 3, 8, 13 };
                            var custinerInx = new int[] { 3, 9, 15 };
                            var statusInx = new int[] { 8, 14, 20 };
                            List<Customer> custList = new List<Customer>();
                            int j = 0;
                            foreach (var pos in custinerInx)
                            {
                                if (string.IsNullOrEmpty(row[pos].ToString()))
                                    continue;

                                var status = row[statusInx[j]].ToString().ToLower();
                                bool onlyTds=false, invalidpan=false,incorrectDob=false, less50L=false, custOptOut=false;

                                if (status.Contains("onlytds")|| status.Contains("only tds"))
                                    onlyTds = true;
                                if (status.Contains("invalidpan") || status.Contains("invalid pan"))
                                    invalidpan = true;
                                if (status.Contains("incorrectdob") || status.Contains("incorrect dob"))
                                    incorrectDob = true;
                                if (status.Contains("less than 50 lakhs") || status.Contains("50"))
                                    less50L = true;
                                if (status.Contains("customer opted out") || status.Contains("customer"))
                                    custOptOut = true;

                                j++;

                                var customer = new Customer
                                {
                                    Name = row[pos].ToString(),
                                    PAN = row[pos + 1].ToString().ToUpper().Trim(),
                                    DateOfBirth = DateTime.ParseExact(row[pos + 2].ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                    MobileNo = row[pos + 3].ToString().Trim(),
                                    EmailID = row[pos + 4].ToString(),
                                    AdressLine1 = row[22].ToString(),
                                    AddressLine2 = row[23].ToString(),
                                    AddressPremises = row[21].ToString(),
                                    City = row[24].ToString(),
                                    PinCode = row[26].ToString(),
                                    StateId = stateObj.StateID,
                                    TracesPassword = "",
                                    AllowForm16B = true,
                                    AlternateNumber = "",
                                    ISD = "+91",
                                    IsPanVerified = false,
                                    IsTracesRegistered = false,
                                    OnlyTDS=onlyTds,
                                    InvalidPAN = invalidpan,
                                    IncorrectDOB = incorrectDob,
                                    LessThan50L = less50L,
                                    CustomerOptedOut = custOptOut
                                };

                                custList.Add(customer);
                            }

                            // save customers
                            Guid guid = Guid.NewGuid();

                            List<decimal> percentage = new List<decimal>();
                            var totlCus = custList.Count;
                            var sharePercent = Convert.ToDecimal((100 / totlCus));
                            var firstShare = sharePercent + (100 - (totlCus * sharePercent));
                            for (var i = 0; i < totlCus; i++)
                            {
                                if (i == 0)
                                    percentage.Add(firstShare);
                                else
                                    percentage.Add(sharePercent);
                            }


                            for (var i = 0; i < custList.Count; i++)
                            {
                                var cus = custList[i];

                                var existCus = _context.Customer.Where(x => x.PAN == cus.PAN).FirstOrDefault();
                                int customerID = 0;
                                if (existCus == null)
                                {
                                    var model = new Customer
                                    {                                        
                                        Name = cus.Name,
                                        PAN = cus.PAN,
                                        DateOfBirth = cus.DateOfBirth,
                                        MobileNo = cus.MobileNo,
                                        EmailID = cus.EmailID,
                                        AdressLine1 = cus.AdressLine1,
                                        AddressLine2 = cus.AddressLine2,
                                        AddressPremises =cus.AddressPremises,
                                        City = cus.City,
                                        PinCode = cus.PinCode,
                                        StateId = cus.StateId,
                                        TracesPassword = "",
                                        AllowForm16B = true,
                                        AlternateNumber = "",
                                        ISD = "+91",
                                        IsPanVerified = false,
                                        IsTracesRegistered = false,
                                        OnlyTDS = cus.OnlyTDS,
                                        InvalidPAN = cus.InvalidPAN,
                                        IncorrectDOB = cus.IncorrectDOB,
                                        LessThan50L = cus.LessThan50L,
                                        CustomerOptedOut = cus.CustomerOptedOut,
                                    };

                                    await _context.Customer.AddAsync(model, cancellationToken);
                                    await _context.SaveChangesAsync(cancellationToken);
                                    customerID = model.CustomerID;
                                }
                                else
                                    customerID = existCus.CustomerID;


                                var val1 = row[28].ToString();
                                var val2 = row[29].ToString();
                                Domain.Entities.CustomerProperty entity = new Domain.Entities.CustomerProperty
                                {
                                    CustomerId = customerID,
                                    CustomerShare = percentage[i],
                                    DateOfSubmission = DateTime.ParseExact(row[0].ToString(), "dd/MM/yyyy",CultureInfo.InvariantCulture),
                                    GstRateID = porperty.GstTaxCode,
                                    IsShared = custList.Count > 1 ? true : false,
                                    PaymentMethodId = 2,
                                    PropertyId = porperty.PropertyID,
                                    Remarks = "",
                                    StatusTypeId = (int)EStatusType.Saved,
                                    TdsCollectedBySeller = true,
                                    TdsRateID = porperty.TDSTaxCode,
                                    TotalUnitCost = Convert.ToDecimal(row[28].ToString().Trim()),
                                    UnitNo = row[2].ToString(),
                                    DateOfAgreement = DateTime.ParseExact(row[27].ToString(),"dd/MM/yyyy", CultureInfo.InvariantCulture),
                                    OwnershipID = guid,
                                    IsArchived = false,
                                    CustomerAlias = custList[0].Name,
                                    StampDuty= Convert.ToDecimal(row[29].ToString().Trim())                                   
                                    //Created = DateTime.Now,
                                    //CreatedBy = userInfo.UserID.ToString()
                                };

                                if (i == 0) {
                                    entity.IsPrimaryOwner = true;
                                }
                                await _context.CustomerProperty.AddAsync(entity, cancellationToken);
                                await _context.SaveChangesAsync(cancellationToken);
                            }

                        }

                        transactionscope.Complete();
                    }
                    catch (Exception ex)
                    {
                        transactionscope.Dispose();
                        // throw new ApplicationException(" Some records are not valid " );
                        throw ex;
                    }
                }

                return true;
            }
        }

    }
}
