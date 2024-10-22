namespace smERP.SharedKernel.ResultDemo;

public interface IResult
{
    public object Value { get; }
    public string Message { get; }
    public int StatusCode { get; }
    public IEnumerable<string> Errors { get; }
}
