using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.Remarks;
using ReProServices.Application.Remarks.Queries;

namespace ReProServices.Application.RegistrationStatus.Queries
{
    public class GetClientPortalListQuery : IRequest<List<ClientPortalDto>>
    {
        public ClientPortalFilter Filter { get; set; } = new ClientPortalFilter();
        public class GetClientPortalListQueryHandler : IRequestHandler<GetClientPortalListQuery, List<ClientPortalDto>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IClientPortalDbContext _portContext;

            private readonly IMapper _mapper;

            public GetClientPortalListQueryHandler(IApplicationDbContext context, IMapper mapper, IClientPortalDbContext portContext)
            {
                _context = context;
                _mapper = mapper;
                _portContext = portContext;
            }

            public async Task<List<ClientPortalDto>> Handle(GetClientPortalListQuery request, CancellationToken cancellationToken)
            {
                var f = request.Filter;
                var portalUser = _portContext.LoginUser.ToList();

                var vm = await (from cs in _context.Customer
                    join cp in _context.CustomerProperty on cs.CustomerID equals cp.CustomerId
                    join py in _context.Property on cp.PropertyId equals py.PropertyID
                    where (f.CustomerId == 0 || f.CustomerId == cp.CustomerId) &&
                          (f.ProjectId == 0 || f.ProjectId == cp.PropertyId) &&
                          (string.IsNullOrEmpty(f.UnitNo) || f.UnitNo == cp.UnitNo)
                    select new ClientPortalDto()
                    {
                        ProjectId = cp.PropertyId,
                        ProjectName = py.AddressPremises,
                        CustomerName = cs.Name,
                        Pan = cs.PAN,
                        CustomerId = cs.CustomerID,
                        UnitNo = cp.UnitNo
                    }).ToListAsync(cancellationToken: cancellationToken);

                var final = (from v in vm
                    join p in portalUser on v.Pan equals p.UserName
                    select new ClientPortalDto()
                    {
                        ProjectId = v.ProjectId,
                        ProjectName = v.ProjectName,
                        CustomerName = v.CustomerName,
                        Pan = v.Pan,
                        CustomerId = v.CustomerId,
                        UnitNo = v.UnitNo,
                        Registered = p.Created,
                        LastUpdated = p.Updated,
                        UserId = p.UserId,
                        Pwd = p.UserPwd
                    }).ToList();

                //var vm = await (from cs in _context.Customer
                //    join cp in _context.CustomerProperty on cs.CustomerID equals cp.CustomerId
                //    join py in _context.Property on cp.PropertyId equals py.PropertyID
                //    join us in _portContext.LoginUser on cs.PAN equals us.UserName into usLeft
                //    from usGrp in usLeft.DefaultIfEmpty()
                //    where (f.CustomerId == 0 || f.CustomerId == cp.CustomerId) &&
                //          (f.ProjectId == 0 || f.ProjectId == cp.PropertyId) &&
                //          (string.IsNullOrEmpty(f.UnitNo) || f.UnitNo == cp.UnitNo)
                //    select new ClientPortalDto()
                //    {
                //        ProjectId = cp.PropertyId,
                //        ProjectName = py.AddressPremises,
                //        CustomerName = cs.Name,
                //        Pan = cs.PAN,
                //        CustomerId = cs.CustomerID,
                //        UnitNo = cp.UnitNo,
                //        Registered = usGrp.Created,
                //        LastUpdated = usGrp.Updated,
                //        UserId = usGrp.UserId,
                //        Pwd = usGrp.UserPwd

                          //    }).ToListAsync(cancellationToken: cancellationToken);

                return final;
            }

        }
    }
   
}
