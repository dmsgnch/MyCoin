using System.Windows.Controls;
using MyCoin.Models;
using MyCoin.ViewModels;

namespace MyCoin.Views;

public partial class CurrencyInfoPage : Page
{
    public readonly CurrencyInfoViewModel CurrencyInfoViewModel;
    
    public CurrencyInfoPage(CurrencyInfoViewModel currencyInfoViewModel)
    {
        InitializeComponent();

        CurrencyInfoViewModel = currencyInfoViewModel;

        DataContext = CurrencyInfoViewModel;
    }
    
    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.InvokeEventThemeChanged();
    }
}