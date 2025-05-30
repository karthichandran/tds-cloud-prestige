using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReProServices.Application.Common.Interfaces;

namespace ReProServices.Application.Remittances.Commands.UpdateRemittance
{
    public class UpdateForm16bReqDatetimeCommand : IRequest<bool>
    {
       
        public int clientTransId { get; set; }
        public DateTime ReqDateTime { get; set; }

        public class UpdateForm16bReqDatetimeCommandHandler : IRequestHandler<UpdateForm16bReqDatetimeCommand, bool>
        {
            private readonly IApplicationDbContext _context;
            public UpdateForm16bReqDatetimeCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(UpdateForm16bReqDatetimeCommand request, CancellationToken cancellationToken)
            {
                var entity = _context.Remittance.FirstOrDefault(x =>
                    x.ClientPaymentTransactionID == request.clientTransId);
                if (entity != null)
                {
                    entity.Form16BReqDatetime = request.ReqDateTime;
                    _context.Remittance.Update(entity);
                }
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
