using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReProServices.Application.Common.Interfaces;
using ReProServices.Application.Customers;
using ReProServices.Application.Customers.Commands.CreateCustomer;
using ReProServices.Domain.Entities;

namespace ReProServices.Application.TransactionLog
{
    public class AddTransactionLogComment : IRequest<bool>
    {
        public TransactionLogDto log { get; set; }

        public class AddTransactionLogCommentHandler : IRequestHandler<AddTransactionLogComment, bool>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            public AddTransactionLogCommentHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
            {
                _context = context;
                _currentUserService = currentUserService;
            }

            public async Task<bool> Handle(AddTransactionLogComment request, CancellationToken cancellationToken)
            {

                var existLog = await _context.TransactionLog.FirstOrDefaultAsync(x =>
                        x.ClientPaymentTransactionId == request.log.ClientPaymentTransactionId,
                    cancellationToken: cancellationToken);
                if (existLog != null)
                {
                    if (!string.IsNullOrEmpty(request.log.Comment))
                        existLog.Comment = request.log.Comment;
                    if (!string.IsNullOrEmpty(request.log.Comment))
                        existLog.ChalanDownload = request.log.ChalanDownload;
                    _context.TransactionLog.Update(existLog);
                }
                else
                {
                    var entity = new Domain.Entities.TransactionLog
                    {
                        ClientPaymentTransactionId = request.log.ClientPaymentTransactionId,
                        Comment = request.log.Comment,
                        ChalanDownload = request.log.ChalanDownload
                    };

                    await _context.TransactionLog.AddAsync(entity, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }
}
