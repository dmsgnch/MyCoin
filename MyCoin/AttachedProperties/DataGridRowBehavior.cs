using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyCoin.AttachedProperties;

public static class DataGridRowBehavior
{
    public static bool GetSelectOnClick(DependencyObject obj)
    {
        return (bool)obj.GetValue(SelectOnClickProperty);
    }

    public static void SetSelectOnClick(DependencyObject obj, bool value)
    {
        obj.SetValue(SelectOnClickProperty, value);
    }

    public static readonly DependencyProperty SelectOnClickProperty =
        DependencyProperty.RegisterAttached("SelectOnClick", typeof(bool), typeof(DataGridRowBehavior), new PropertyMetadata(false, OnSelectOnClickChanged));

    private static void OnSelectOnClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGridRow row)
        {
            if ((bool)e.NewValue)
            {
                row.PreviewMouseLeftButtonDown += Row_PreviewMouseLeftButtonDown;
            }
            else
            {
                row.PreviewMouseLeftButtonDown -= Row_PreviewMouseLeftButtonDown;
            }
        }
    }

    private static void Row_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGridRow row)
        {
            if (!row.IsSelected)
            {
                row.IsSelected = true;
                var dataGrid = FindParent<DataGrid>(row);
                if (dataGrid is not null)
                {
                    dataGrid.SelectedItem = row.DataContext;
                }
            }
        }
    }

    private static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);

        if (parentObject == null) return null;

        if (parentObject is T parent)
        {
            return parent;
        }
        else
        {
            return FindParent<T>(parentObject);
        }
    }
}