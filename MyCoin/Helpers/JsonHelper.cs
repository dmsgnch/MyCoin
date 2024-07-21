using System.Data;
using System.Net.Http;
using Newtonsoft.Json;

namespace MyCoin.Helpers;

public static class JsonHelper
{
    /// <summary>
    /// Reads http response and deserializes it into an object of the specified type
    /// </summary>
    /// <param name="res">Http response object</param>
    /// <typeparam name="T">Deserialize type</typeparam>
    /// <returns>Generic type deserialized value</returns>
    public static async Task<T> GetTypeFromResponseAsync<T>(HttpResponseMessage res)
    {
        var jsonString = await res.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonString) ??
               throw new DataException("Json deserialize error!");
    }
}