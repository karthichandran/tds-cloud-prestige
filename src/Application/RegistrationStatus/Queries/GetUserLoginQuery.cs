using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using System.Xml.Serialization;

namespace ReProServices.Application.RegistrationStatus.Queries
{
    public class GetUserLoginQuery : IRequest<ClientPortalDto>
    {
        public string PanNumber { get; set; }

        public class GetUserLoginQueryHandler : IRequestHandler<GetUserLoginQuery, ClientPortalDto>
        {
            private readonly IApplicationDbContext _context;
            private readonly IClientPortalDbContext _portContext;

            public GetUserLoginQueryHandler(IApplicationDbContext context, IClientPortalDbContext portContext)
            {
                _context = context;
                _portContext = portContext;
            }

            public async Task<ClientPortalDto> Handle(GetUserLoginQuery request, CancellationToken cancellationToken)
            {
                var user =await _portContext.LoginUser.FirstOrDefaultAsync(x => x.UserName == request.PanNumber);
                if (user == null)
                    return null;


                var model = new ClientPortalDto()
                {
                    UserId = user.UserId,
                    Pan = user.UserName,
                    Pwd = user.UserPwd
                };
                return model;
            }

        }
    }
}
