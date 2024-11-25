using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.RegistrationStatus.Comments;
using ReProServices.Domain.Entities.ClientPortal;

namespace ReProServices.Application.InfoContent
{
    public class SaveInfoContentCommand : IRequest<bool>
    {
        public InfoContentDto info { get; set; }

        public class SaveInfoContentCommandHandler : IRequestHandler<SaveInfoContentCommand, bool>
        {
            private readonly IApplicationDbContext _context;

            public SaveInfoContentCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(SaveInfoContentCommand request, CancellationToken cancellationToken)
            {
                var info =  _context.InfoContent.FirstOrDefault(x => x.PossessionUnit == request.info.PossessionUnit);

                if (info == null)
                {
                    var entity = new Domain.Entities.ClientPortal.InfoContent()
                    {
                        ProfileTxt = request.info.ProfileTxt,
                        PaymentToSeller = request.info.PaymentToSeller,
                        Form16B = request.info.Form16B,
                        LoginPopUp = request.info.LoginPopUp,
                        Faq = request.info.Faq,
                        TdsCompliance = request.info.TdsCompliance,
                        PossessionUnit = request.info.PossessionUnit
                    };
                    await _context.InfoContent.AddAsync(entity, cancellationToken);
                }
                else
                {
                    info.ProfileTxt = request.info.ProfileTxt;
                    info.PaymentToSeller = request.info.PaymentToSeller;
                    info.Form16B = request.info.Form16B;
                    info.LoginPopUp = request.info.LoginPopUp;
                    info.Faq = request.info.Faq;
                    info.TdsCompliance = request.info.TdsCompliance;
                  
                     _context.InfoContent.Update(info);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
