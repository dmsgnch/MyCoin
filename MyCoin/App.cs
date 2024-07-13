using System.Windows;
using MyCoin.Views;

namespace MyCoin;

public class App : Application
{
    private readonly CurrencyListWindow _currencyListWindow;
    
    public App(CurrencyListWindow currencyListWindow)
    {
        _currencyListWindow = currencyListWindow;
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        _currencyListWindow.Show();  
        base.OnStartup(e);
    }
}