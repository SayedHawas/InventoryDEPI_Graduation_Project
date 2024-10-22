using MediatR;
using Microsoft.AspNetCore.Mvc;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.WebApi.Controllers;

public class AppControllerBase : ControllerBase
{
    private IMediator _mediatorInstance;
    protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();

    #region Actions

    //protected ObjectResult CustomResult(Result response)
    //{
    //    return GetObjectResult(response);
    //}

    // Overload for handling List responses
    //public ObjectResult CustomResult<T>(Result<List<T>> response)
    //{
    //    return GetObjectResult(response);
    //}

    //public ObjectResult CustomResult<T>(PaginatedResult<T> response)
    //{
    //    return GetObjectResult(response);
    //}

    //// Overload for handling List responses
    //public ObjectResult CustomResult<T>(PaginatedResult<List<T>> response)
    //{
    //    return GetObjectResult(response);
    //}
    //private ObjectResult GetObjectResult<T>(PaginatedResult<T> response)
    //{
    //    switch (response.StatusCode)
    //    {
    //        case HttpStatusCode.OK:
    //            return new OkObjectResult(response);
    //        case HttpStatusCode.Unauthorized:
    //            return new UnauthorizedObjectResult(response);
    //        case HttpStatusCode.BadRequest:
    //            return new BadRequestObjectResult(response);
    //        case HttpStatusCode.NotFound:
    //            return new NotFoundObjectResult(response);
    //        case HttpStatusCode.Accepted:
    //            return new AcceptedResult(string.Empty, response);
    //        case HttpStatusCode.UnprocessableEntity:
    //            return new UnprocessableEntityObjectResult(response);
    //        default:
    //            return new BadRequestObjectResult(response);
    //    }
    //}
    //private ObjectResult GetObjectResult(Result response)
    //{
    //    switch (response.StatusCode)
    //    {
    //        case HttpStatusCode.OK:
    //            return new OkObjectResult(response);
    //        case HttpStatusCode.Created:
    //            return new CreatedResult(string.Empty, response);
    //        case HttpStatusCode.Unauthorized:
    //            return new UnauthorizedObjectResult(response);
    //        case HttpStatusCode.BadRequest:
    //            return new BadRequestObjectResult(response);
    //        case HttpStatusCode.NotFound:
    //            return new NotFoundObjectResult(response);
    //        case HttpStatusCode.Accepted:
    //            return new AcceptedResult(string.Empty, response);
    //        case HttpStatusCode.UnprocessableEntity:
    //            return new UnprocessableEntityObjectResult(response);
    //        default:
    //            return new BadRequestObjectResult(response);
    //    }
    //}

    #endregion
}
