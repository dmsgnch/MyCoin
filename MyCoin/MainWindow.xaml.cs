using System.Windows;
using System.Windows.Media;
using MyCoin.Views;
using System.Windows.Navigation;

namespace MyCoin;

public partial class MainWindow : Window
{
    public MainWindow(CurrencyListPage currencyListPage)
    {
        InitializeComponent();
        
        App.ThemeChanged += OnThemeChanged;
        
        MainFrame.Navigate(currencyListPage);
    }
    
    private void OnThemeChanged()
    {
        UpdateResources(this);
    }
    
    /// <summary>
    /// Performs resource dictionary prescribing and state updates for all inheritors
    /// </summary>
    /// <param name="obj">UI element</param>
    private void UpdateResources(DependencyObject obj)
    {
        if (obj is FrameworkElement element)
        {
            element.Resources.MergedDictionaries.Clear();
            foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
            {
                element.Resources.MergedDictionaries.Add(dictionary);
            }

            element.InvalidateVisual();
            element.UpdateLayout();
        }

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            UpdateResources(VisualTreeHelper.GetChild(obj, i));
        }
    }
    
    private void MainFrame_Navigated(object sender, NavigationEventArgs e)
    {
        OnThemeChanged();
    }
}