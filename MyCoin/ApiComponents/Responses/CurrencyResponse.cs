using MyCoin.Models;
using Newtonsoft.Json;

namespace MyCoin.ApiComponents.Responses;

public class CurrencyResponse
{
    [JsonProperty("data")] 
    public Currency Data { get; set; } = new();
}