using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MyCoin.ApiComponents.Requests;
using MyCoin.ApiComponents.Responses;
using MyCoin.ApiComponents.Routes;
using MyCoin.Commands;
using MyCoin.Components;
using MyCoin.Helpers;
using MyCoin.Models;
using MyCoin.Services;
using MyCoin.Services.Abstract;
using MyCoin.Views;

namespace MyCoin.ViewModels;

public class CurrencyListViewModel : INotifyPropertyChanged
{
    private readonly HttpClientServiceBase _httpClientService;
    private readonly IThemeChanger _themeChanger;

    public ICommand UpdateCurrenciesCommand { get; }
    public ICommand OpenCurrencyInfoCommand { get; }
    public ICommand ChangeThemeCommand { get; }

    private ObservableCollection<Currency> _currencies;

    public ObservableCollection<Currency> Currencies
    {
        get => _currencies;
        set
        {
            if (_currencies != value)
            {
                _currencies = value;
                SetFilteredCurrencies();
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<Currency> _filteredCurrencies;

    public ObservableCollection<Currency> FilteredCurrencies
    {
        get => _filteredCurrencies;
        set
        {
            if (_filteredCurrencies != value)
            {
                _filteredCurrencies = value;
                OnPropertyChanged();
            }
        }
    }

    private object? _selectedItem;
    public object? SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            ((RelayCommandAsync)OpenCurrencyInfoCommand).RaiseCanExecuteChanged();
            OnPropertyChanged();
        }
    }

    private string _stringFilter;
    public string StringFilter
    {
        get => _stringFilter;
        set
        {
            _stringFilter = value;
            SetFilteredCurrencies();
            OnPropertyChanged();
        }
    }

    public CurrencyListViewModel(HttpClientServiceBase httpClientService, IThemeChanger themeChanger)
    {
        _httpClientService = httpClientService;
        _themeChanger = themeChanger;

        UpdateCurrenciesCommand = new RelayCommandAsync(async (param) => await UpdateCurrenciesAsync());
        OpenCurrencyInfoCommand =
            new RelayCommandAsync(async (param) => await OpenCurrencyInfoPageAsync(), IsItemSelected);
        ChangeThemeCommand = new RelayCommandAsync(async (param) => await ChangeThemeAsync(), IsThemeCanBeChange);

        UpdateCurrenciesCommand.Execute(null);
    }

    private void SetFilteredCurrencies()
    {
        if (String.IsNullOrEmpty(StringFilter))
        {
            FilteredCurrencies = new ObservableCollection<Currency>(Currencies);
        }
        else
        {
            FilteredCurrencies =
                new ObservableCollection<Currency>(
                    Currencies.Where(c => c.Name.Length >= StringFilter.Length && c.Name.ToLower().StartsWith(StringFilter.ToLower()) 
                                           || c.Symbol.Length >= StringFilter.Length && c.Symbol.ToLower().StartsWith(StringFilter.ToLower())).ToList());
        }
    }

    #region Update currencies command

    private async Task UpdateCurrenciesAsync()
    {
        var rowResponse = await _httpClientService.ProcessRequestAsync(new HttpRequestForm(
            endPoint: ApiRoutes.CoinCapRoutes.CoinCapUrlBase + ApiRoutes.CoinCapRoutes.GetAllCurrencies,
            requestMethod: HttpMethod.Get));

        if (rowResponse.IsSuccessStatusCode)
        {
            UpdateCurrenciesList((await JsonHelper.GetTypeFromResponseAsync<CurrencyListResponse>(rowResponse)).Data);
        }
        else
        {
            UpdateCurrenciesList(new List<Currency>());
            await HandleMessageAsync((await JsonHelper.GetTypeFromResponseAsync<ErrorResponse>(rowResponse)).ErrorInfo);
        }
    }

    private void UpdateCurrenciesList(List<Currency> currencies)
    {
        Currencies = new ObservableCollection<Currency>(currencies);
    }

    private async Task HandleMessageAsync(string messageText)
    {
        Message message = new Message(messageText, "Error");
        MessageChain messageChain = new MessageChain(message, true, false, false);

        MessageHandlerBase displayToUserHandler = new DisplayToUserMessageHandler();

        await displayToUserHandler.HandleAsync(messageChain);
    }

    #endregion

    #region Open currency info page command

    private async Task OpenCurrencyInfoPageAsync()
    {
        if (SelectedItem is Currency selectedCurrency)
        {
            var currencyInfoPage = App.ServiceProvider.GetRequiredService<CurrencyInfoPage>();

            currencyInfoPage.CurrencyInfoViewModel.SelectedCurrency = selectedCurrency;
            
            currencyInfoPage.CurrencyInfoViewModel.UpdateMultiCommand.Execute(null);
            
            ((App)Application.Current).OpenNewPage(currencyInfoPage);
        }
    }

    private bool IsItemSelected() => SelectedItem is not null;

    #endregion

    #region Change theme command
    
    private async Task ChangeThemeAsync()
    {
        _themeChanger.ChangeTheme();

        FilteredCurrencies = new ObservableCollection<Currency>(FilteredCurrencies);
    }

    private bool IsThemeCanBeChange() => _themeChanger.IsThemeCanBeChange();

    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}