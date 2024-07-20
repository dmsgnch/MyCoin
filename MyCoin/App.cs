using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using MyCoin;

namespace MyCoin;

public class App : Application
{
    private readonly MainWindow _mainWindow;
    public static event Action ThemeChanged;
    
    public static IServiceProvider ServiceProvider { get; set; }
    
    public App(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        ApplyTheme("/Resources/ResourcesDictionaries/Themes/LightThemeStyles.xaml");
        ApplyTheme("/Resources/ResourcesDictionaries/MainStyles.xaml");
        ApplyTheme("/Resources/ResourcesDictionaries/CurrencyListStyles.xaml");
        
        _mainWindow.Show(); 
    }
    
    private void ApplyTheme(string themePath)
    {
        var theme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
        
        Resources.MergedDictionaries.Add(theme);
    }

    public static void InvokeEventThemeChanged()
    {
        ThemeChanged?.Invoke();
    }

    public void OpenNewPage(Page page)
    {
        _mainWindow.MainFrame.Navigate(page);
    }
}