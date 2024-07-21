using MyCoin.Models;
using Newtonsoft.Json;

namespace MyCoin.ApiComponents.Responses;

/// <summary>
/// Class for correct List of Markets data retrieval when receiving a response from the API
/// </summary>
public class MarketsListResponse
{
    [JsonProperty("data")] 
    public List<Market> Data { get; set; } = new();
}