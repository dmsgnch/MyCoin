namespace MyCoin.ApiComponents.Routes;

internal static class ApiRoutes
{
    internal static class CoinCapRoutes
    {
        internal const string CoinCapUrlBase = "https://api.coincap.io/v2/";
        internal const string GetAllCurrencies = "assets/";
        internal static string GetMarketsByCoinId(string id) => $"assets/{id}/markets/";
    }
    
    internal static class CoinGeckoRoutes
    {
        internal const string CoinGeckoUrlBase = "https://api.coingecko.com/api/v3/";
        internal const string ApiKey = "CG-434asjZRdbkxLkdSNshHFxzW";
        internal static string GetCoinByCoinId(string id) => $"coins/{id}";
    }
}