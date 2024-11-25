using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.RegistrationStatus;
using ReProServices.Application.RegistrationStatus.Queries;

namespace ReProServices.Application.InfoContent
{
    public class GetInfoContentQuery : IRequest<InfoContentDto>
    {
        public bool PossessionUnit { get; set; }
        public class GetInfoContentQueryHandler : IRequestHandler<GetInfoContentQuery, InfoContentDto>
        {
            private readonly IApplicationDbContext _context;
            public GetInfoContentQueryHandler(IApplicationDbContext context, IClientPortalDbContext portContext)
            {
                _context = context;
            }

            public async Task<InfoContentDto> Handle(GetInfoContentQuery request, CancellationToken cancellationToken)
            {

                var info = await _context.InfoContent.FirstOrDefaultAsync(x=>x.PossessionUnit==request.PossessionUnit);
                if (info == null)
                {
                    return new InfoContentDto()
                    {
                        PossessionUnit = request.PossessionUnit
                    };
                }

                return new InfoContentDto()
                {
                    PossessionUnit = info.PossessionUnit,
                    ProfileTxt = info.ProfileTxt,
                    PaymentToSeller = info.PaymentToSeller,
                    Form16B = info.Form16B,
                    LoginPopUp = info.LoginPopUp,
                    Faq = info.Faq,
                    TdsCompliance = info.TdsCompliance
                };
            }

        }
    }
}
