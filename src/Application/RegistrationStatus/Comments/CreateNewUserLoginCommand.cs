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
    public class CreateNewUserLoginCommand : IRequest<bool>
    {
        public List<(string,string)> PanList { get; set; }

        public class CreateNewUserLoginCommandHandler : IRequestHandler<CreateNewUserLoginCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly IClientPortalDbContext _portContext;

            public CreateNewUserLoginCommandHandler(IApplicationDbContext context, IClientPortalDbContext portContext)
            {
                _context = context;
                _portContext = portContext;
            }

            public async Task<bool> Handle(CreateNewUserLoginCommand request, CancellationToken cancellationToken)
            {
                var chars = "#@$*0";
                List<LoginUser> userList = new List<LoginUser>();
                request.PanList.ForEach(pan =>
                {
                    var isExist= _portContext.LoginUser.Any(x => x.UserName==pan.Item1);
                    if (!isExist)
                    {
                        var pwd = ShuffleString(pan.Item1 + chars);
                        userList.Add(new LoginUser
                        {
                            UserName = pan.Item1,
                            Email = pan.Item2,
                            UserPwd = pwd,
                            IsActive = true
                        });
                    }
                });

                await _portContext.LoginUser.AddRangeAsync(userList, cancellationToken);
                await _portContext.SaveChangesAsync(cancellationToken);

                return true;
            }

            private static string ShuffleString(string str)
            {
                char[] array = str.ToCharArray();
                Random rand = new Random();

                // Fisher-Yates shuffle algorithm
                for (int i = array.Length - 1; i > 0; i--)
                {
                    int j = rand.Next(0, i + 1);
                    // Swap array[i] with array[j]
                    char temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }

                // Convert the shuffled array back to a string
                string shuffled = new string(array);

                // Append special characters
                return shuffled;
            }

        }
    }
}
