using System;
using AutoMapper;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReProServices.Domain.Entities;
using System.Collections.Generic;

namespace ReProServices.Application.Customers.Queries
{
    public class GetCustomersQuery : IRequest<CustomerVM>
    {
        public CustomerDetailsFilter Filter { get; set; } = new CustomerDetailsFilter();
        public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, CustomerVM>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<GetCustomersQueryHandler> _logger;
            public GetCustomersQueryHandler(IApplicationDbContext context, IMapper mapper, ILogger<GetCustomersQueryHandler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<CustomerVM> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
            {
                CustomerVM customerVM = new CustomerVM();
                List<ViewCustomerPropertyBasic> cpList;
                if (request.Filter.StatusTypeId == 7) {
                    cpList = _context.ViewCustomerPropertyArchived.ToList()
                   .GroupBy(g => g.OwnershipID)
                   .Select(x => new ViewCustomerPropertyBasic
                   {
                       PropertyPremises = x.First().PropertyPremises,
                       CustomerName = string.Join(",", x.Select(g => g.CustomerName)),
                       PAN = string.Join(",", x.Select(g => g.PAN)),
                       DateOfAgreement = x.First().DateOfAgreement,
                       OwnershipID = x.First().OwnershipID,
                       UnitNo = x.First().UnitNo,
                       CustomerID = x.First().CustomerID,
                       CustomerPropertyID = x.First().CustomerPropertyID,
                       DateOfSubmission = x.First().DateOfSubmission,
                       PaymentMethodId = x.First().PaymentMethodId,
                       PropertyID = x.First().PropertyID,
                       Remarks = x.First().Remarks,
                       StatusTypeID = x.First().StatusTypeID,
                       TotalUnitCost = x.First().TotalUnitCost,
                       TracesPassword = string.Join(",", x.Select(g => g.TracesPassword)),
                       CustomerAlias=x.First().CustomerAlias,
                       UnitStatus=x.First().UnitStatus,
                       IsPanVerified= string.Join(",", x.Select(g => g.IsPanVerified))
                   }).AsQueryable()
                  .FilterCustomersBy(request.Filter).ToList();
                }
                else
                 cpList = _context.ViewCustomerPropertyBasic.ToList()
                    .GroupBy(g => g.OwnershipID)
                    .Select(x => new ViewCustomerPropertyBasic
                    {
                        PropertyPremises = x.First().PropertyPremises,
                        CustomerName = string.Join(",", x.Select(g => g.CustomerName)),
                        PAN = string.Join(",", x.Select(g => g.PAN)),
                        DateOfAgreement = x.First().DateOfAgreement,
                        OwnershipID = x.First().OwnershipID,
                        UnitNo = x.First().UnitNo,
                        CustomerID = x.First().CustomerID,
                        CustomerPropertyID = x.First().CustomerPropertyID,
                        DateOfSubmission = x.First().DateOfSubmission,
                        PaymentMethodId = x.First().PaymentMethodId,
                        PropertyID = x.First().PropertyID,
                        Remarks = x.First().Remarks,
                        StatusTypeID = x.First().StatusTypeID,
                        TotalUnitCost = x.First().TotalUnitCost,
                        TracesPassword= string.Join(",", x.Select(g => g.TracesPassword)),
                        CustomerAlias= x.First().CustomerAlias,
                        UnitStatus = x.First().UnitStatus,
                        IsPanVerified = string.Join(",", x.Select(g => g.IsPanVerified))
                    }).AsQueryable()         
                   .FilterCustomersBy(request.Filter).ToList();

                try
                {
                    var withoutProp = _context.ViewCustomerWithoutProperty
                        .Select(x => new ViewCustomerPropertyBasic
                        {
                            CustomerName = x.CustomerName,
                            PAN = x.PAN,
                            CustomerID = x.CustomerID,
                            IsPanVerified=x.IsPanVerified
                        })
                        .ToList()
                        .AsQueryable().FilterCustomersBy(request.Filter).ToList();

                  cpList.AddRange(withoutProp);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                customerVM.customersView = cpList;
                return customerVM;
            }
        }
    }
}

