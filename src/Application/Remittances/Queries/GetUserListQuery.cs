using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.ClientPayments;
using ReProServices.Application.Common.Interfaces;

namespace ReProServices.Application.Remittances.Queries
{
    public class GetUserListQuery:IRequest<List<DropDownDto>>
    {
        public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, List<DropDownDto>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public GetUserListQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<DropDownDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
            {
                var userList =await _context.ClientPaymentTransaction.Where(x=>!string.IsNullOrEmpty( x.PaymentBy))
                    .Select(x => new DropDownDto() {ID = x.PaymentBy, UserName = x.PaymentBy}).Distinct().ToListAsync();
               
                return userList;
            }
        }
    }
}
