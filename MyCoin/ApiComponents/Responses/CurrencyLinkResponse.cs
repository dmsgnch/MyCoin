using Newtonsoft.Json;

namespace MyCoin.ApiComponents.Responses;

public class CurrencyLinkResponse
{
    [JsonProperty("links")] 
    public Links CurrencyLinks { get; set; }

    public class Links
    {
        [JsonProperty("homepage")] 
        public List<string> Homepage { get; set; }
    }
}