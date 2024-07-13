using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyCoin.Views;

namespace MyCoin;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<App>();
                services.AddSingleton<CurrencyListWindow>();
            })
            .Build();
        var app = host.Services.GetService<App>();
        app?.Run();
    }
}