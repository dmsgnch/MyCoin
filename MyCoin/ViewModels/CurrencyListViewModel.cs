using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MyCoin.ApiComponents.Requests;
using MyCoin.ApiComponents.Responses;
using MyCoin.ApiComponents.Routes;
using MyCoin.Commands;
using MyCoin.Components;
using MyCoin.Helpers;
using MyCoin.Models;
using MyCoin.Services;
using MyCoin.Services.Abstract;
using Newtonsoft.Json;

namespace MyCoin.ViewModels;

public class CurrencyListViewModel : INotifyPropertyChanged
{
    private readonly HttpClientServiceBase _httpClientService;
    
    public ICommand UpdateCurrenciesCommand { get; }
    public ICommand OpenCurrencyInfoCommand { get; }
    
    private ObservableCollection<Currency> _currencies;
    public ObservableCollection<Currency> Currencies
    {
        get => _currencies;
        set
        {
            if (_currencies != value)
            {
                _currencies = value;
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

    public CurrencyListViewModel(HttpClientServiceBase httpClientService)
    {
        _httpClientService = httpClientService;
        UpdateCurrenciesCommand = new RelayCommandAsync(async (param) => await UpdateCurrenciesAsync());
        OpenCurrencyInfoCommand = new RelayCommandAsync(async (param) => await OpenCurrencyInfoPageAsync(), IsItemSelected);
        
        UpdateCurrenciesCommand.Execute(null);
    }

    private async Task UpdateCurrenciesAsync()
    {
        var rowResponse = await _httpClientService.ProcessRequestAsync(new HttpRequestForm(
            endPoint: ApiRoutes.CoinCapRoutes.CoinCapUrlBase + ApiRoutes.CoinCapRoutes.GetAllCurrencies,
            requestMethod: HttpMethod.Get));
        
        if (rowResponse.IsSuccessStatusCode)
        {
            UpdateCurrenciesList((await JsonHelper.GetTypeFromResponseAsync<CurrencyResponse>(rowResponse)).Data);
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

    private async Task OpenCurrencyInfoPageAsync()
    {
        if (SelectedItem is Currency selectedCurrency)
        {
            MessageBox.Show($"Selected row rank: {selectedCurrency.Rank}");
        }
    }

    private bool IsItemSelected() => SelectedItem is not null;
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}