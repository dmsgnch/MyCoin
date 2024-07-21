using System.IO;
using System.Windows;
using MyCoin.Services.Abstract;

namespace MyCoin.Services;

public class ThemeChanger : IThemeChanger
{
    private const string ProjectDictionariesPath = @"/Resources/ResourcesDictionaries/Themes/";
    
    /// <summary>
    /// Execute application theme changes using themes from the theme directory
    /// </summary>
    public void ChangeTheme()
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
    
    public bool IsThemeCanBeChange() => GetListOfFileNamesInDirectory(ProjectDictionariesPath).Count > 1;
}