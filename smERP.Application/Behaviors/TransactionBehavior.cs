using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.MarkerInterfaces;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response;

        //try
        //{
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
        throw new Exception();
            response = await next();
            await _unitOfWork.CommitAsync(cancellationToken);
        //}
        //catch (Exception exception)
        //{
        //    await _unitOfWork.RollbackAsync(cancellationToken);

        //    var result = new Result<bool>()
        //        .WithError(SharedResourcesKeys.DatabaseError)
        //        .WithMessage(HttpStatusCode.InternalServerError.ToString())
        //        .WithStatusCode(HttpStatusCode.InternalServerError);

        //    return (TResponse)result;
        //}

        return response;
    }
}