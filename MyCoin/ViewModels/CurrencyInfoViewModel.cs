using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
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

namespace MyCoin.ViewModels;

public class CurrencyInfoViewModel : INotifyPropertyChanged
{
    private readonly HttpClientServiceBase _httpClientService;
    private readonly IThemeChanger _themeChanger;

    public ICommand UpdateCurrencyCommand { get; }
    public ICommand ChangeThemeCommand { get; }
    public ICommand UpdateMarketsCommand { get; }
    public ICommand UpdateMultiCommand { get; }

    private Currency _selectedCurrency;

    public Currency SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            _selectedCurrency = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Market> _currencyMarkets;

    public ObservableCollection<Market> CurrencyMarkets
    {
        get => _currencyMarkets;
        set
        {
            _currencyMarkets = value;
            OnPropertyChanged();
        }
    }

    public CurrencyInfoViewModel(HttpClientServiceBase httpClientService, IThemeChanger themeChanger)
    {
        _httpClientService = httpClientService;
        _themeChanger = themeChanger;

        UpdateCurrencyCommand = new RelayCommandAsync(async (param) => await UpdateCurrencyAsync());
        ChangeThemeCommand = new RelayCommandAsync(async (param) => await ChangeTheme(), IsThemeCanBeChange);
        UpdateMarketsCommand = new RelayCommandAsync(async (param) => await UpdateMarketsAsync());
        UpdateMultiCommand = new RelayCommandAsync(async (param) => await MultiUpdateAsync());
    }

    private async Task MultiUpdateAsync()
    {
        UpdateCurrencyCommand.Execute(null);
        UpdateMarketsCommand.Execute(null);
    }

    #region Update currency command

    private async Task UpdateCurrencyAsync()
    {
        var rowResponse = await _httpClientService.ProcessRequestAsync(new HttpRequestForm(
            endPoint: ApiRoutes.CoinCapRoutes.CoinCapUrlBase + ApiRoutes.CoinCapRoutes.GetAllCurrencies +
                      SelectedCurrency.Id,
            requestMethod: HttpMethod.Get));

        if (rowResponse.IsSuccessStatusCode)
        {
            UpdateCurrency((await JsonHelper.GetTypeFromResponseAsync<CurrencyResponse>(rowResponse)).Data);
        }
        else
        {
            UpdateCurrency(new Currency());
            await HandleMessageAsync((await JsonHelper.GetTypeFromResponseAsync<ErrorResponse>(rowResponse)).ErrorInfo);
        }
    }

    private void UpdateCurrency(Currency currency)
    {
        SelectedCurrency = currency;
    }

    private async Task HandleMessageAsync(string messageText)
    {
        Message message = new Message(messageText, "Error");
        MessageChain messageChain = new MessageChain(message, true, false, false);

        MessageHandlerBase displayToUserHandler = new DisplayToUserMessageHandler();

        await displayToUserHandler.HandleAsync(messageChain);
    }

    #endregion

    #region Change theme command

    private async Task ChangeTheme()
    {
        _themeChanger.ChangeTheme();
    }

    private bool IsThemeCanBeChange() => _themeChanger.IsThemeCanBeChange();

    #endregion

    #region Update markets command

    private async Task UpdateMarketsAsync()
    {
        var rowResponse = await _httpClientService.ProcessRequestAsync(new HttpRequestForm(
            endPoint: ApiRoutes.CoinCapRoutes.CoinCapUrlBase +
                      ApiRoutes.CoinCapRoutes.GetMarketsByCoinId(SelectedCurrency.Id),
            requestMethod: HttpMethod.Get));

        if (rowResponse.IsSuccessStatusCode)
        {
            UpdateMarkets((await JsonHelper.GetTypeFromResponseAsync<MarketsListResponse>(rowResponse)).Data);
        }
        else
        {
            UpdateMarkets(new List<Market>());
            await HandleMessageAsync((await JsonHelper.GetTypeFromResponseAsync<ErrorResponse>(rowResponse)).ErrorInfo);
        }
    }

    private void UpdateMarkets(List<Market> markets)
    {
        CurrencyMarkets = new ObservableCollection<Market>(markets);
    }

    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}