using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.Remittances.Commands.UpdateRemittance;

namespace ReProServices.Application.Remittances.Commands.UpdateRemittance
{
   public class UpdateAttemptsCommand : IRequest<bool>
    {
        public int clientPayId { get; set; }
        public int bankAcctId { get; set; }

        public class UpdateAttemptsCommandHandler : IRequestHandler<UpdateAttemptsCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            public UpdateAttemptsCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(UpdateAttemptsCommand request, CancellationToken cancellationToken)
            {

                var entity = _context.ClientPaymentTransaction.FirstOrDefault(x =>
                    x.ClientPaymentTransactionID == request.clientPayId);
                if (entity != null)
                {
                    if (entity.NoOfAttempst == null)
                        entity.NoOfAttempst = 1;
                    else
                    entity.NoOfAttempst = entity.NoOfAttempst+1;
                    if (request.bankAcctId > 0)
                        entity.BankAcctId = request.bankAcctId;
                    _context.ClientPaymentTransaction.Update(entity);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
