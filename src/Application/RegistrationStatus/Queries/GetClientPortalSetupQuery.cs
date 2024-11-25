using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;

namespace ReProServices.Application.RegistrationStatus.Queries
{
    public class GetClientPortalSetupQuery : IRequest<ClientPortalSetupDto>
    {
      
        public class GetClientPortalSetupQueryHandler : IRequestHandler<GetClientPortalSetupQuery, ClientPortalSetupDto>
        {
            private readonly IApplicationDbContext _context;
          
           

            public GetClientPortalSetupQueryHandler(IApplicationDbContext context,  IClientPortalDbContext portContext)
            {
                _context = context;
            }

            public async Task<ClientPortalSetupDto> Handle(GetClientPortalSetupQuery request, CancellationToken cancellationToken)
            {

                var dropdownList =await _context.ClientPortalSetup.FromSqlRaw("exec sp_GetClientPortalSetup").ToListAsync(cancellationToken);

                var propertyList= dropdownList.Where(x=>x.Category==1).Select(s => new { s.Id, s.PropertyName })
                    .ToList()
                    .Select(s => (s.Id, s.PropertyName)).ToList();
                var customerList = dropdownList.Where(x => x.Category == 2).Select(s => new { s.Id, s.PropertyName })
                    .ToList()
                    .Select(s => (s.Id, s.PropertyName)).ToList();
                var unitNoList = dropdownList.Where(x => x.Category == 3).Select(s => new { s.Id, s.PropertyName })
                    .ToList()
                    .Select(s => (s.PropertyName, s.PropertyName)).ToList();

                var model = new ClientPortalSetupDto()
                {
                    ProjectName = propertyList,
                    CustomerName = customerList,
                    UnitNo = unitNoList
                };
                return model;
            }

        }
    }
}
