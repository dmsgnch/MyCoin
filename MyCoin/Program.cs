using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyCoin.Services;
using MyCoin.Services.Abstract;
using MyCoin.ViewModels;
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
                services.AddSingleton<MainWindow>();
                services.AddSingleton<CurrencyListViewModel>();
                services.AddSingleton<CurrencyListPage>();

                services.AddHttpClient("MyHttpClient");
                services.AddTransient<HttpClientServiceBase, HttpClientService>();
            })
            .Build();
        var app = host.Services.GetService<App>();
        app?.Run();
    }
}