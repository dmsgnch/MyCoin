namespace MyCoin.ApiComponents.Responses;

public class ErrorResponse
{
    public string ErrorInfo { get; set; }

    public ErrorResponse(string errorInfo)
    {
        ErrorInfo = errorInfo;
    }
}