using System.Net.Http;

namespace MyCoin.ApiComponents.Requests;

/// <summary>
/// Saves all the data to send http request
/// </summary>
public class HttpRequestForm
{
    public string EndPoint { get; }
    public HttpMethod RequestMethod { get; }
    public string? ApiKey { get; set; }
    public string? JsonData { get; }

    public HttpRequestForm(string endPoint,
        HttpMethod requestMethod,
        string? apiKey = null,
        string? jsonData = null)
    {
        EndPoint = endPoint;
        RequestMethod = requestMethod;
        ApiKey = apiKey;
        JsonData = jsonData;
    }
}