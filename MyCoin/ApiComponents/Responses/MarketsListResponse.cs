using MyCoin.Models;
using Newtonsoft.Json;

namespace MyCoin.ApiComponents.Responses;

public class MarketsListResponse
{
    [JsonProperty("data")] 
    public List<Market> Data { get; set; } = new();
}