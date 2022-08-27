﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Domain.Entities;

namespace ReProServices.Application.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest<CustomerVM>
    {
        public CustomerVM CustomerVM { get; set; }

        public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerVM>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public UpdateCustomerCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<CustomerVM> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var userInfo = _context.Users.FirstOrDefault(x => x.UserName == _currentUserService.UserName && x.IsActive);
                    foreach (var customer in request.CustomerVM.customers)
                    {

                        //todo duplicate code in create customer too. need to revert and cleanup
                        //Check if customer already exist
                        if (customer.CustomerID > 0) //todo check for usage of nullable customerId
                        {
                            Customer entityUpd = new Customer
                            {
                                CustomerID = customer.CustomerID,
                                AdressLine1 = customer.AdressLine1,
                                AddressLine2 = customer.AddressLine2,
                                AddressPremises = customer.AddressPremises,
                                City = customer.City,
                                DateOfBirth = customer.DateOfBirth.Date,
                                EmailID = customer.EmailID,
                                IsTracesRegistered = customer.IsTracesRegistered,
                                MobileNo = customer.MobileNo,
                                Name = customer.Name,
                                PAN = customer.PAN,
                                PinCode = customer.PinCode.Trim(),
                                StateId = customer.StateId,
                                TracesPassword = customer.TracesPassword,
                                AllowForm16B = customer.AllowForm16B,
                                AlternateNumber = customer.AlternateNumber,
                                ISD = customer.ISD,
                                IsPanVerified = customer.IsPanVerified.Value,
                                OnlyTDS = customer.OnlyTDS,
                                InvalidPAN = customer.InvalidPAN,
                                IncorrectDOB = customer.IncorrectDOB,
                                LessThan50L = customer.LessThan50L,
                                CustomerOptedOut = customer.CustomerOptedOut
                                //Updated = DateTime.Now,
                                //UpdatedBy = userInfo.UserID.ToString()
                            };
                            _context.Customer.Update(entityUpd);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                        else
                        {
                            Customer entity = new Customer
                            {
                                AdressLine1 = customer.AdressLine1,
                                AddressLine2 = customer.AddressLine2,
                                AddressPremises = customer.AddressPremises,
                                City = customer.City,
                                DateOfBirth = customer.DateOfBirth.Date,
                                EmailID = customer.EmailID,
                                IsTracesRegistered = customer.IsTracesRegistered,
                                MobileNo = customer.MobileNo,
                                Name = customer.Name,
                                PAN = customer.PAN,
                                PinCode = customer.PinCode.Trim(),
                                StateId = customer.StateId,
                                TracesPassword = customer.TracesPassword,
                                AllowForm16B = customer.AllowForm16B,
                                AlternateNumber = customer.AlternateNumber,
                                ISD = customer.ISD,
                                IsPanVerified = customer.IsPanVerified.Value,
                                OnlyTDS = customer.OnlyTDS,
                                InvalidPAN = customer.InvalidPAN,
                                IncorrectDOB = customer.IncorrectDOB,
                                LessThan50L = customer.LessThan50L,
                                CustomerOptedOut = customer.CustomerOptedOut
                                //Created = DateTime.Now,
                                //CreatedBy = userInfo.UserID.ToString()
                            };
                            _ = await _context.Customer.AddAsync(entity, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);
                            customer.CustomerID = entity.CustomerID;
                        }
                    }

                    CustomerVM customerVM = new CustomerVM
                    {
                        customers = request.CustomerVM.customers
                    };
                    return customerVM;
                }
                catch(Exception ex)
                {
                    if (ex.InnerException.Message.Contains("Violation of UNIQUE KEY constraint 'UQ_Customer_PAN'")) {

                        throw new ApplicationException("PAN already exists");
                    }
                    throw ex;

                }
            }
        }
    }
}
