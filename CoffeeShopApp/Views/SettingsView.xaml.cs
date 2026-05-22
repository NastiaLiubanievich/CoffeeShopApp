using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CoffeeShopApp.Services;

namespace CoffeeShopApp;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        CurrentUserTextBlock.Text = App.CurrentUserName;
        CurrentRoleTextBlock.Text = App.CurrentUserRole;
        UpdateHeaderColors();
        UpdateThemeButtons();
    }

    private void LightThemeButton_Click(object sender, RoutedEventArgs e)
    {
        AppSettings.SetTheme("Світла");
        UpdateHeaderColors();
        UpdateThemeButtons();
    }

    private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
    {
        AppSettings.SetTheme("Темна");
        UpdateHeaderColors();
        UpdateThemeButtons();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        App.CurrentUserName = "Адміністратор";
        App.CurrentUserRole = "Адміністратор";

        var loginWindow = new LoginWindow();
        Application.Current.MainWindow = loginWindow;
        loginWindow.Show();

        Window.GetWindow(this)?.Close();
    }

    private void UpdateThemeButtons()
    {
        SetThemeButtonState(LightThemeButton, AppSettings.UseLightTheme);
        SetThemeButtonState(DarkThemeButton, !AppSettings.UseLightTheme);
    }

    private void UpdateHeaderColors()
    {
        if (AppSettings.UseLightTheme)
        {
            TitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(23, 18, 14));
            SubtitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(95, 88, 80));
            return;
        }

        TitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(252, 250, 247));
        SubtitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(216, 199, 179));
    }

    private static void SetThemeButtonState(Button button, bool isActive)
    {
        button.Background = isActive
            ? new SolidColorBrush(Color.FromRgb(255, 250, 244))
            : Brushes.White;
        button.Foreground = new SolidColorBrush(Color.FromRgb(59, 44, 29));
        button.BorderBrush = isActive
            ? new SolidColorBrush(Color.FromRgb(122, 74, 31))
            : new SolidColorBrush(Color.FromRgb(231, 222, 210));
        button.BorderThickness = new Thickness(isActive ? 2 : 1);
    }
}
