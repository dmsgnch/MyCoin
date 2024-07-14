using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using MyCoin.ApiComponents.Responses;
using MyCoin.ApiComponents.Requests;
using MyCoin.Services.Abstract;
using Newtonsoft.Json;

namespace MyCoin.Services;

public class HttpClientService : HttpClientServiceBase
{
    private readonly IHttpClientFactory _clientFactory;
    private const string CoinCapUrlBase = "https://api.coincap.io/v2/";

    public HttpClientService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    protected override HttpRequestMessage CreateHttpRequest(HttpRequestForm requestForm)
    {
        var client = _clientFactory.CreateClient("ApiClient");

        var request = new HttpRequestMessage(requestForm.RequestMethod, CoinCapUrlBase + requestForm.EndPoint);

        if (requestForm.JsonData != null)
        {
            request.Content = new StringContent(requestForm.JsonData, Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrWhiteSpace(requestForm.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", requestForm.Token);
        }

        return request;
    }
    
    protected override async Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.SendAsync(request);
        return response;
    }
    
    protected override HttpResponseMessage HandleResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return response;
        }
        else
        {
            return new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(new ErrorResponse(
                        "An unexpected error occurred on the server. Please try again! " +
                        "If the problem persists, please contact support.")))
            };
        }
    }
}