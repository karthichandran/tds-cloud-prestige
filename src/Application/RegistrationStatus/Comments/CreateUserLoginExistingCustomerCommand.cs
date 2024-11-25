using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.Customers;
using ReProServices.Application.Customers.Commands.CreateCustomer;
using ReProServices.Domain.Entities.ClientPortal;

namespace ReProServices.Application.RegistrationStatus.Comments
{
    public class CreateUserLoginExistingCustomerCommand : IRequest<bool>
    {
        public class CreateUserLoginExistingCustomerCommandHandler : IRequestHandler<CreateUserLoginExistingCustomerCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly IClientPortalDbContext _portContext;

            public CreateUserLoginExistingCustomerCommandHandler(IApplicationDbContext context, IClientPortalDbContext portContext)
            {
                _context = context;
                _portContext = portContext;
            }

            public async Task<bool> Handle(CreateUserLoginExistingCustomerCommand request, CancellationToken cancellationToken)
            {
                List<string> existPan = _portContext.LoginUser.Select(x => x.UserName).ToList();

               // var newPans = _context.Customer.Where(x => !existPan.Contains(x.PAN)).Select(s=>s.PAN).ToList();
                var newPans = _context.Customer.Where(x => !existPan.Contains(x.PAN)).Select(s => new{ s.PAN,s.EmailID}).ToList();

                var chars = "#@$*0";
                List<LoginUser> userList = new List<LoginUser>();
                newPans.ForEach(pan =>
                {
                    var pwd = ShuffleString(pan.PAN + chars);
                    userList.Add(new LoginUser
                    {
                        UserName = pan.PAN,
                        Email = pan.EmailID,
                        UserPwd = pwd,
                        IsActive = true
                    });

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
