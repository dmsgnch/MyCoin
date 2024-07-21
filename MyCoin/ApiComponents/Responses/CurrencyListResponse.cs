using MyCoin.Models;
using Newtonsoft.Json;

namespace MyCoin.ApiComponents.Responses;

public class CurrencyListResponse
{
    [JsonProperty("data")] 
    public List<Currency> Data { get; set; } = new();
}