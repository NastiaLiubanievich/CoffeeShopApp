using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CoffeeShopApp.Services;

namespace CoffeeShopApp;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();

        ConfirmCheckBox.IsChecked = AppSettings.ShowOrderConfirmation;
        LightThemeCheckBox.IsChecked = AppSettings.UseLightTheme;
        ApplyTheme();
    }

    private void ConfirmCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            AppSettings.ShowOrderConfirmation = checkBox.IsChecked == true;
        }
    }

    private void LightThemeCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            AppSettings.UseLightTheme = checkBox.IsChecked == true;
            ApplyTheme();
        }
    }

    private void ApplyTheme()
    {
        Background = AppSettings.UseLightTheme
            ? new SolidColorBrush(Color.FromRgb(248, 246, 242))
            : new SolidColorBrush(Color.FromRgb(49, 39, 25));
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
