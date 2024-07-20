using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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

public class CurrencyListViewModel : INotifyPropertyChanged
{
    private const string ProjectDictionariesPath = @"/Resources/ResourcesDictionaries/Themes/";

    private readonly HttpClientServiceBase _httpClientService;

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

    public CurrencyListViewModel(HttpClientServiceBase httpClientService)
    {
        _httpClientService = httpClientService;

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

    #endregion

    #region Open currency info page command

    private async Task OpenCurrencyInfoPageAsync()
    {
        if (SelectedItem is Currency selectedCurrency)
        {
            MessageBox.Show($"Selected row rank: {selectedCurrency.Rank}");
        }
    }

    private bool IsItemSelected() => SelectedItem is not null;

    #endregion

    #region Change theme command

    /// <summary>
    /// Execute application theme changes using themes from the theme directory
    /// </summary>
    private async Task ChangeThemeAsync()
    {
        //Get list of themes (relative path)
        var themesList = GetListOfFileNamesInDirectory(ProjectDictionariesPath);

        //Get current theme
        var currentTheme = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(rd =>
                themesList.Contains(rd.Source?.OriginalString
                                    ?? throw new Exception("Source is null")));

        if (currentTheme is null) throw new Exception("No theme styles found to remove!");

        //Find the next theme for applying in the list of themes
        var nextTheme = GetNextResourceDictionary(themesList, currentTheme);

        Application.Current.Resources.MergedDictionaries.Remove(currentTheme);
        Application.Current.Resources.MergedDictionaries.Add(nextTheme);

        //Invoke the event that will update all UI elements of the window
        App.InvokeEventThemeChanged();

        FilteredCurrencies = new ObservableCollection<Currency>(FilteredCurrencies);
    }

    /// <summary>
    /// Method for dynamic loading of theme files
    /// </summary>
    /// <returns>String array of file names starting from the root directory of the project</returns>
    private List<string> GetListOfFileNamesInDirectory(string resourceDictionariesPath)
    {
        //Get the directory of the project executable file
        var executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //Get the root directory by going up through the parents
        var projectRootDirectory = Directory.GetParent(executableDirectory)?.Parent?.Parent?.Parent;

        if (projectRootDirectory is null)
            throw new DirectoryNotFoundException(
                "Error of finding the parent item when getting the root directory of a project");

        var combinedPath = projectRootDirectory.FullName + resourceDictionariesPath;
        //Get the list of files in the directory with full paths
        var themesWithFullPaths =
            Directory.GetFiles(combinedPath, "*.xaml");
        //Get the list of files in the directory with relative paths
        return themesWithFullPaths.Select(filePath => resourceDictionariesPath + Path.GetFileName(filePath)).ToList();
    }

    /// <summary>
    /// Select next theme in file
    /// </summary>
    /// <returns>ResourceDictionary that should be applied by the following</returns>
    private ResourceDictionary GetNextResourceDictionary(List<string> themesList, ResourceDictionary currentTheme)
    {
        var currentIndex = themesList.IndexOf(currentTheme.Source?.OriginalString
                                              ?? throw new Exception("Source is null"));
        var nextIndex = (currentIndex + 1) % themesList.Count;

        return new ResourceDictionary
            { Source = new Uri(themesList[nextIndex], UriKind.RelativeOrAbsolute) };
    }

    private bool IsThemeCanBeChange() => GetListOfFileNamesInDirectory(ProjectDictionariesPath).Count > 1;

    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}