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
                
                services.AddTransient<CurrencyListViewModel>();
                services.AddTransient<CurrencyListPage>();
                
                services.AddTransient<CurrencyInfoViewModel>();
                services.AddTransient<CurrencyInfoPage>();

                services.AddHttpClient("MyHttpClient");
                services.AddTransient<HttpClientServiceBase, HttpClientService>();

                services.AddTransient<IThemeChanger, ThemeChanger>();
            })
            .Build();
        var app = host.Services.GetService<App>();
        App.ServiceProvider = host.Services;
        app?.Run();
    }
}