using System.Net;
using System.Net.Http;
using MyCoin.ApiComponents.Requests;
using MyCoin.ApiComponents.Responses;
using Newtonsoft.Json;

namespace MyCoin.Services.Abstract;

public abstract class HttpClientServiceBase
{
    public async Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestForm requestForm)
    {
        try
        {
            var request = CreateHttpRequest(requestForm);
            var response = await SendHttpRequestAsync(request);
            return HandleResponse(response);
        }
        catch (HttpRequestException httpEx)
        {
            return new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(new ErrorResponse(
                        "An unexpected error occurred while trying to access the server. Please try again! " +
                        "If the problem persists, please contact support.")))
            };
        }
        catch (TaskCanceledException taskEx)
        {
            return new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(new ErrorResponse("The server is not responding")))
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred: {ex.Message}");
        }
    }

    protected abstract HttpRequestMessage CreateHttpRequest(HttpRequestForm requestForm);
    protected abstract Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request);
    protected abstract HttpResponseMessage HandleResponse(HttpResponseMessage response);
}