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
        ApplyTheme("LightTheme.xaml");

        _currencyListWindow.Show();  
        base.OnStartup(e);
    }

    private void ApplyTheme(string themePath)
    {
        var theme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
        
        Resources.MergedDictionaries.Clear();
        
        Resources.MergedDictionaries.Add(theme);
    }
}