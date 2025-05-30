using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.Remittances.Commands.UpdateRemittance;

namespace ReProServices.Application.TdsRemittance.Commands
{
   public class UpdateBankAndUserDetailsCommand : IRequest<bool>
    {
        public List<TdsPaymentDto> TdsPayList { get; set; }

        public class UpdateBankAndUserDetailsCommandHandler : IRequestHandler<UpdateBankAndUserDetailsCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            public UpdateBankAndUserDetailsCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(UpdateBankAndUserDetailsCommand request, CancellationToken cancellationToken)
            {
                var items = request.TdsPayList;
                foreach (var obj in items)
                {
                    var entity = _context.ClientPaymentTransaction.FirstOrDefault(x =>
                        x.ClientPaymentTransactionID == obj.ClientPaymentTransactionID);
                    if (entity != null)
                    {
                        entity.PaymentBy = obj.PaymentBy;
                        entity.ExpectedPaymentDate = obj.expectedPaymentDate;
                        entity.BankAcctId = obj.BankAcctId;
                        _context.ClientPaymentTransaction.Update(entity);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
