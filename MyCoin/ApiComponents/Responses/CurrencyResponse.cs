using MyCoin.Models;
using Newtonsoft.Json;

namespace MyCoin.ApiComponents.Responses;

/// <summary>
/// Class for correct Currency data retrieval when receiving a response from the API
/// </summary>
public class CurrencyResponse
{
    [JsonProperty("data")] 
    public Currency Data { get; set; } = new();
}