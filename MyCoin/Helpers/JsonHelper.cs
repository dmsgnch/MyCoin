using System.Data;
using System.Net.Http;
using Newtonsoft.Json;

namespace MyCoin.Helpers;

public static class JsonHelper
{
    public static async Task<T> GetTypeFromResponseAsync<T>(HttpResponseMessage res)
    {
        var jsonString = await res.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonString) ??
               throw new DataException("Json deserialize error!");
    }
}