using MyCoin.Models;
using Newtonsoft.Json;

namespace MyCoin.ApiComponents.Responses;

/// <summary>
/// Class for correct List of Currencies data retrieval when receiving a response from the API
/// </summary>
public class CurrencyListResponse
{
    [JsonProperty("data")] 
    public List<Currency> Data { get; set; } = new();
}