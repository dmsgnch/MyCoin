using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using MyCoin.Services.Abstract;
using MyCoin.ApiComponents.Requests;
using MyCoin.ApiComponents.Responses;
using Newtonsoft.Json;

namespace MyCoin.Services;

public class HttpClientService : HttpClientServiceBase
{
    private readonly IHttpClientFactory _clientFactory;

    public HttpClientService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    protected override HttpRequestMessage CreateHttpRequest(HttpRequestForm requestForm)
    {
        var client = _clientFactory.CreateClient("MyHttpClient");

        var request = new HttpRequestMessage(requestForm.RequestMethod, requestForm.EndPoint);

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
                        "Api service error. Please try again!")))
            };
        }
    }
}