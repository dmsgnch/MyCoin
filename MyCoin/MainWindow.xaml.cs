using System.Windows;
using MyCoin.ViewModels;
using MyCoin.Views;

namespace MyCoin;

public partial class MainWindow : Window
{
    public MainWindow(CurrencyListViewModel currencyListViewModel)
    {
        InitializeComponent();
        MainFrame.Navigate(new CurrencyListPage(currencyListViewModel));
    }
}