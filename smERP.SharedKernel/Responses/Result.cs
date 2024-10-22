using FluentResults;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using System.Net;

namespace smERP.SharedKernel.Responses;

public class Result<T> : FluentResults.Result<T>, IResult<T>
{
    private HttpStatusCode _statusCode = HttpStatusCode.OK;
    private string _message = SharedResourcesKeys.Success.Localize();

    public HttpStatusCode StatusCode
    {
        get => _statusCode;
        set
        {
            _statusCode = value;
        }
    }

    public string Message
    {
        get => _message;
        set
        {
            _message = value;
        }
    }

    public Result() : base()
    {
        UpdateStatusAndMessage(HttpStatusCode.OK);
    }

    public Result(T value) : base()
    {
        WithValue(value);
        UpdateStatusAndMessage(HttpStatusCode.OK);
    }


    public new Result<T> WithError(IError error)
    {
        base.WithError(error);
        UpdateStatusAndMessage(HttpStatusCode.BadRequest);
        return this;
    }
    public new Result<T> WithError(string errorMessage)
    {
        base.WithError(errorMessage);
        return this;
    }

    public new Result<T> WithErrors(IEnumerable<string> errors)
    {
        base.WithErrors(errors);
        return this;
    }

    public new Result<T> WithErrors(IEnumerable<IError> errors)
    {
        base.WithErrors(errors);
        return this;
    }

    public new Result<T> WithSuccess(ISuccess success)
    {
        base.WithSuccess(success);
        return this;
    }

    public async Task<IResultBase> WithTask(Func<Task> task)
    {
        try
        {
            await task();
            return this;
        }
        catch (Exception exception)
        {
            base.WithError(new Error(exception.Message).CausedBy(exception));
            UpdateStatusAndMessage(HttpStatusCode.InternalServerError);
            return this;
        }
    }

    public async Task<IResultBase> WithTask(Func<Task> task, string errorMessageIfFail)
    {
        try
        {
            await task();
            return this;
        }
        catch (Exception exception)
        {
            base.WithError(new Error(errorMessageIfFail.Localize()).CausedBy(exception));
            UpdateStatusAndMessage(HttpStatusCode.InternalServerError);
            return this;
        }
    }

    public IResult<T> WithStatusCode(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        return this;
    }

    public IResult<T> WithMessage(string message)
    {
        Message = message;
        return this;
    }


    public IResult<TNewValue> ChangeType<TNewValue>(TNewValue value)
    {
        var result = new Result<TNewValue>(value);
        result.StatusCode = this.StatusCode;
        result.Message = this.Message;
        result.WithReasons(this.Reasons);

        return result;
    }

    private void UpdateStatusAndMessage(HttpStatusCode statusCode)
    {

        StatusCode = statusCode;
        Message = statusCode.ToString().Localize();
    }

    public Result<TNewValue> Bind<TNewValue>(Func<T, Result<TNewValue>> bind)
    {
        var result = new Result<TNewValue>();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = bind(Value);
            result.WithValue(converted.ValueOrDefault);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    public IResultBase WithOk()
    {
        StatusCode = HttpStatusCode.OK;
        Message = SharedResourcesKeys.Success.Localize();
        return this;
    }

    public IResultBase WithCreated()
    {
        StatusCode = HttpStatusCode.Created;
        Message = SharedResourcesKeys.Created.Localize();
        return this;
    }

    public IResultBase WithUpdated()
    {
        StatusCode = HttpStatusCode.Accepted;
        Message = SharedResourcesKeys.Updated.Localize();
        return this;
    }

    public IResultBase WithBadRequest(string? errorMessage = null)
    {
        StatusCode = HttpStatusCode.BadRequest;
        Message = SharedResourcesKeys.BadRequest.Localize();
        if (errorMessage is null)
            return this;
        return WithError(errorMessage);
    }
    public IResultBase WithDeleted()
    {
        StatusCode = HttpStatusCode.NoContent;
        Message = SharedResourcesKeys.DeletedSuccess.Localize();
        return this;
    }

    public IResult<T> WithNotFound(string? errorMessage = null)
    {
        StatusCode = HttpStatusCode.NotFound;
        Message = SharedResourcesKeys.NotFound.Localize();
        if (errorMessage is null)
            return this;
        return WithError(errorMessage);
    }

    public IResultBase WithBadRequest(object value)
    {
        throw new NotImplementedException();
    }

    public IResult<T> WithBadRequestResult(string? errorMessage)
    {
        StatusCode = HttpStatusCode.BadRequest;
        Message = SharedResourcesKeys.BadRequest.Localize();
        if (errorMessage is null)
            return this;
        return WithError(errorMessage);
    }

    public IResult<T> WithBadRequestResult(IEnumerable<string> errorMessages)
    {
        StatusCode = HttpStatusCode.BadRequest;
        Message = SharedResourcesKeys.BadRequest.Localize();
        if (errorMessages is null)
            return this;
        return WithErrors(errorMessages);
    }

    IResultBase IResultBase.WithErrors(IEnumerable<string> errorMessages)
    {
        StatusCode = HttpStatusCode.BadRequest;
        Message = SharedResourcesKeys.BadRequest.Localize();
        base.WithErrors(errorMessages);
        return this;
    }
}

// Non-generic version of Result for compatibility
//public class Result : IResult
//{
//    public HttpStatusCode StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//    public string Message { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//    public bool IsFailed => throw new NotImplementedException();

//    public bool IsSuccess => throw new NotImplementedException();

//    public List<IReason> Reasons => throw new NotImplementedException();

//    public List<IError> Errors => throw new NotImplementedException();

//    public List<ISuccess> Successes => throw new NotImplementedException();

//    public Result() : base() { }

//    public static Result<T> Ok<T>(T value)
//    {
//        return new Result<T>(value);
//    }

//    public static Result<T> Fail<T>(string errorMessage)
//    {
//        return new Result<T>().WithError(new Error(errorMessage));
//    }

//    public static Result<T> Fail<T>(IError error)
//    {
//        return new Result<T>().WithError(error);
//    }

//    public static Result<T> Fail<T>(List<IError> errors)
//    {
//        var result = new Result<T>();
//        foreach (var error in errors)
//        {
//            result.WithError(error);
//        }
//        return result;
//    }

//    public IResult WithStatusCode(HttpStatusCode statusCode)
//    {
//        throw new NotImplementedException();
//    }

//    public IResult WithMessage(string message)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<IResultBase> WithTask(Func<Task> task)
//    {
//        throw new NotImplementedException();
//    }
//}
public interface IResultBase : FluentResults.IResultBase
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }
    public IResultBase WithOk();
    public IResultBase WithCreated();
    public IResultBase WithUpdated();
    public IResultBase WithBadRequest(string? errorMessage);
    public IResultBase WithDeleted();
    public IResultBase WithErrors(IEnumerable<string> errorMessages);
    public Task<IResultBase> WithTask(Func<Task> task);
    public Task<IResultBase> WithTask(Func<Task> task, string errorMessageIfFail);
}

public interface IResult : IResultBase
{
    public IResult WithStatusCode(HttpStatusCode statusCode);
    public IResult WithMessage(string message);
}

public interface IResult<T> : IResultBase
{
    public T Value { get; }
    public IResult<T> WithStatusCode(HttpStatusCode statusCode);
    public IResult<T> WithMessage(string message);
    public IResult<T> WithNotFound(string? errorMessage);
    //public new IResult<T> WithCreated();
    //public new IResult<T> WithUpdated();
    public IResult<T> WithBadRequestResult(string? errorMessage);
    public IResult<T> WithBadRequestResult(IEnumerable<string> errorMessages);
    //public new IResult<T> WithDeleted();
    public IResult<TNewValue> ChangeType<TNewValue>(TNewValue value);
}