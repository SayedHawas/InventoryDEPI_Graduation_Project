using FluentValidation;
using MediatR;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResultBase
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures.Select(f => f.ErrorMessage).ToList();

            var genericArguments = typeof(TResponse).GetGenericArguments();

            if (genericArguments.Length > 0)
            {
                var resultType = typeof(Result<>).MakeGenericType(genericArguments[0]);
                var GenericResult = (IResultBase)Activator.CreateInstance(resultType);

                GenericResult.WithErrors(errors);

                return (TResponse)GenericResult;
            }

            var result = new Result<bool>()
                .WithErrors(errors)
                .WithMessage(SharedResourcesKeys.BadRequest.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);

            return (TResponse)result;
        }

        return await next();
    }
}

//public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//        where TRequest : IRequest<TResponse>
//        where TResponse : IResultBase
//{
//    private readonly IEnumerable<IValidator<TRequest>> _validators;
//    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
//    {
//        _validators = validators;
//    }

//    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        var context = new ValidationContext<TRequest>(request);
//        var failures = _validators
//            .Select(x => x.Validate(context))
//            .SelectMany(x => x.Errors)
//            .Where(x => x != null)
//            .ToList();

//        if (failures.Count != 0)
//        {
//            var errors = failures.Select(f => f.ErrorMessage).ToList();

//            var result = new Result<bool>()
//                .WithErrors(errors)
//                .WithMessage(SharedResourcesKeys.BadRequest.Localize())
//                .WithStatusCode(HttpStatusCode.BadRequest);

//            return (TResponse)result;

//        }
//        return await next();
//    }
//}