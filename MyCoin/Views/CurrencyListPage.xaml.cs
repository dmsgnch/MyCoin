using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MyCoin.ViewModels;

namespace MyCoin.Views;

public partial class CurrencyListPage : Page
{
    public CurrencyListPage(CurrencyListViewModel currencyListViewModel)
    {
        InitializeComponent();
        
        DataContext = currencyListViewModel;
    }
    
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid dataGrid && dataGrid.SelectedItem is not null)
        {
            var viewModel = DataContext as CurrencyListViewModel;
            if (viewModel is null) throw new Exception("DataContext is not view model");
            
            if (viewModel.OpenCurrencyInfoCommand.CanExecute(null))
            {
                viewModel.OpenCurrencyInfoCommand.Execute(dataGrid.SelectedItem);
            }
        }
    }
    
    private void DataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        DependencyObject originalSource = e.OriginalSource as DependencyObject;

        while (originalSource != null && !(originalSource is DataGridColumnHeader))
        {
            originalSource = VisualTreeHelper.GetParent(originalSource);
        }

        if (originalSource is DataGridColumnHeader)
        {
            e.Handled = true;
        }
    }
}