using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Domain.Entities.ClientPortal;

namespace ReProServices.Application.RegistrationStatus.Comments
{
    public class UpdateUserPasswordCommand : IRequest<bool>
    {
        public UserLoginModel UserModel { get; set; }

        public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly IClientPortalDbContext _portContext;

            public UpdateUserPasswordCommandHandler(IApplicationDbContext context, IClientPortalDbContext portContext)
            {
                _context = context;
                _portContext = portContext;
            }

            public async Task<bool> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = _portContext.LoginUser.FirstOrDefault(x => x.UserId == request.UserModel.UserId);
                if (user == null)
                    return false;

                user.UserPwd = request.UserModel.Pwd;
                _portContext.LoginUser.Update(user);
                await _portContext.SaveChangesAsync(cancellationToken);
                return true;
            }

        }
    }
}
