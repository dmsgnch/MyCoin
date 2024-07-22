using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    public ICommand UpdateCurrencyLinkCommand { get; }
    public ICommand UpdateMultiCommand { get; }

    #region Params
    
    private string _currencyLink;
    public string CurrencyLink
    {
        get => _currencyLink;
        set
        {
            _currencyLink = value;
            OnPropertyChanged();
        }
    }
    
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
    
    #endregion

    public CurrencyInfoViewModel(HttpClientServiceBase httpClientService, IThemeChanger themeChanger)
    {
        _httpClientService = httpClientService;
        _themeChanger = themeChanger;

        UpdateCurrencyCommand = new RelayCommandAsync(async (param) => await UpdateCurrencyAsync());
        ChangeThemeCommand = new RelayCommandAsync(async (param) => await ChangeTheme(), IsThemeCanBeChange);
        UpdateMarketsCommand = new RelayCommandAsync(async (param) => await UpdateMarketsAsync());
        UpdateCurrencyLinkCommand = new RelayCommandAsync(async (param) => await UpdateCurrencyLinkAsync());
        UpdateMultiCommand = new RelayCommandAsync(async (param) => await MultiUpdateAsync());
    }

    private async Task MultiUpdateAsync()
    {
        UpdateCurrencyCommand.Execute(null);
        UpdateMarketsCommand.Execute(null);
        UpdateCurrencyLinkCommand.Execute(null);
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
        MessageChain messageChain = new MessageChain(message, true, false, true);

        MessageHandlerBase displayToUserHandler = new DisplayToUserMessageHandler();
        MessageHandlerBase writeToConsoleHandler = new WriteToConsoleMessageHandler(App.ServiceProvider.GetRequiredService<ILogger<CurrencyInfoViewModel>>());

        displayToUserHandler.Successor = writeToConsoleHandler;
        
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
        CurrencyMarkets = new ObservableCollection<Market>(markets.Where(m => !m.ExchangeId.Equals("Gate")));
    }

    #endregion
    
    #region Update currency link command

    private async Task UpdateCurrencyLinkAsync()
    {
        var rowResponse = await _httpClientService.ProcessRequestAsync(new HttpRequestForm(
            endPoint: ApiRoutes.CoinGeckoRoutes.CoinGeckoUrlBase + ApiRoutes.CoinGeckoRoutes.GetCoinByCoinId(SelectedCurrency.Id),
            apiKey: ApiRoutes.CoinGeckoRoutes.ApiKey,
            requestMethod: HttpMethod.Get));

        if (rowResponse.IsSuccessStatusCode)
        {
            UpdateCurrencyLink((await JsonHelper.GetTypeFromResponseAsync<CurrencyLinkResponse>(rowResponse)).CurrencyLinks.Homepage);
        }
        else
        {
            UpdateCurrencyLink(new List<string>{new string("No data")});
            await HandleMessageAsync((await JsonHelper.GetTypeFromResponseAsync<ErrorResponse>(rowResponse)).ErrorInfo);
        }
    }

    private void UpdateCurrencyLink(List<string> links)
    {
        CurrencyLink = links[0];
    }

    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}