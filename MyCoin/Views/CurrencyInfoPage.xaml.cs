using System.Windows.Controls;
using MyCoin.Models;
using MyCoin.ViewModels;

namespace MyCoin.Views;

public partial class CurrencyInfoPage : Page
{
    public CurrencyInfoPage(CurrencyInfoViewModel currencyInfoViewModel)
    {
        InitializeComponent();

        DataContext = currencyInfoViewModel;
    }
}