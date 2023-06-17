using MediatR;
using ReProServices.Application.Common.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using ReProServices.Domain.Entities;
using System.Linq;
using System;

namespace ReProServices.Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<CustomerVM>
    {
        public CustomerVM CustomerVM { get; set; }

        public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerVM>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public CreateCustomerCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<CustomerVM> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                var userInfo = _context.Users.FirstOrDefault(x => x.UserName == _currentUserService.UserName && x.IsActive);
                foreach (var customer in request.CustomerVM.customers)
                {
                    //Check if customer already exist
                    if (customer.CustomerID > 0) //todo check for usage of nullable customerId
                    {
                        Customer entityUpd = new Customer
                        {
                            CustomerID = customer.CustomerID,
                            //AdressLine1 = customer.AdressLine1,
                            //AddressLine2 = customer.AddressLine2,
                            //AddressPremises = customer.AddressPremises,
                            //City = customer.City,
                            DateOfBirth = customer.DateOfBirth.Date,
                            EmailID = customer.EmailID,
                            IsTracesRegistered = customer.IsTracesRegistered,
                            //MobileNo = customer.MobileNo,
                            Name = customer.Name,
                            PAN = customer.PAN,
                            //PinCode = customer.PinCode,
                            //StateId = customer.StateId,
                            TracesPassword = customer.TracesPassword,
                            //AllowForm16B = customer.AllowForm16B,
                            //AlternateNumber=customer.AlternateNumber,
                            //ISD=customer.ISD,
                            IsPanVerified= customer.IsPanVerified.Value,
                            OnlyTDS=customer.OnlyTDS,
                            InvalidPAN = customer.InvalidPAN,
                            IncorrectDOB = customer.IncorrectDOB,
                            LessThan50L = customer.LessThan50L,
                            CustomerOptedOut = customer.CustomerOptedOut,
                            CustomerOptingOutDate = customer.CustomerOptingOutDate,
                            CustomerOptingOutRemarks = customer.CustomerOptingOutRemarks,
                            InvalidPanDate = customer.InvalidPanDate,
                            InvalidPanRemarks = customer.InvalidPanRemarks,
                            IncomeTaxPassword=customer.IncomeTaxPassword
                            //Updated = DateTime.Now,
                            //UpdatedBy = userInfo.UserID.ToString()
                        };
                        if (entityUpd.OnlyTDS != true)
                        {
                            entityUpd.InvalidPanDate = null;
                            entityUpd.InvalidPanRemarks = "";
                        }

                        if (entityUpd.CustomerOptedOut != true)
                        {
                            entityUpd.CustomerOptingOutDate = null;
                            entityUpd.CustomerOptingOutRemarks = "";
                        }
                        _context.Customer.Update(entityUpd);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    else
                    {
                        Customer entity = new Customer
                        {
                            //AdressLine1 = customer.AdressLine1,
                            //AddressLine2 = customer.AddressLine2,
                            //AddressPremises = customer.AddressPremises,
                            //City = customer.City,
                            DateOfBirth = customer.DateOfBirth.Date,
                            EmailID = customer.EmailID,
                            IsTracesRegistered = customer.IsTracesRegistered,
                            //MobileNo = customer.MobileNo,
                            Name = customer.Name,
                            PAN = customer.PAN,
                            //PinCode = customer.PinCode,
                            //StateId = customer.StateId,
                            TracesPassword = customer.TracesPassword,
                            //AllowForm16B = customer.AllowForm16B,
                            //AlternateNumber = customer.AlternateNumber,
                            //ISD = customer.ISD,
                            IsPanVerified = customer.IsPanVerified.Value,
                            OnlyTDS = customer.OnlyTDS,
                            InvalidPAN = customer.InvalidPAN,
                            IncorrectDOB = customer.IncorrectDOB,
                            LessThan50L = customer.LessThan50L,
                            CustomerOptedOut = customer.CustomerOptedOut,
                            CustomerOptingOutDate = customer.CustomerOptingOutDate,
                            CustomerOptingOutRemarks = customer.CustomerOptingOutRemarks,
                            InvalidPanDate = customer.InvalidPanDate,
                            InvalidPanRemarks = customer.InvalidPanRemarks,
                            IncomeTaxPassword = customer.IncomeTaxPassword
                            //Created = DateTime.Now,
                            //CreatedBy = userInfo.UserID.ToString()
                        };

                        if (entity.OnlyTDS != true)
                        {
                            entity.InvalidPanDate = null;
                            entity.InvalidPanRemarks = "";
                        }

                        if (entity.CustomerOptedOut != true)
                        {
                            entity.CustomerOptingOutDate = null;
                            entity.CustomerOptingOutRemarks = "";
                        }

                        await _context.Customer.AddAsync(entity, cancellationToken);
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
        }
    }
}
