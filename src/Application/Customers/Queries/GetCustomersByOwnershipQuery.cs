using AutoMapper;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ReProServices.Application.Customers.Queries
{
    public class GetCustomersByOwnershipQuery : IRequest<List<CustomerDto>>
    {
        public Guid OwnershipId { get; set; }
        public class GetCustomersByOwnershipQueryHandler : IRequestHandler<GetCustomersByOwnershipQuery, List<CustomerDto>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;
            public GetCustomersByOwnershipQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<CustomerDto>> Handle(GetCustomersByOwnershipQuery request, CancellationToken cancellationToken)
            {
                var vm = await _context.Customer
                    .Include(cp => cp.CustomerProperty)
                    .Where(cp => cp.CustomerProperty.Any(c => c.OwnershipID == request.OwnershipId))
                    .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return vm;
            }
        }
    }
}
