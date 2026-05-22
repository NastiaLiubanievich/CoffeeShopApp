using System.Windows;
using System.Windows.Controls;
using CoffeeShopApp.Services;

namespace CoffeeShopApp;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        SelectComboBoxItem(LanguageComboBox, AppSettings.InterfaceLanguage);
        SelectComboBoxItem(TimeZoneComboBox, AppSettings.TimeZone);
        SelectComboBoxItem(CurrencyComboBox, AppSettings.Currency);
        SelectComboBoxItem(ThemeComboBox, AppSettings.ThemeName);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        AppSettings.InterfaceLanguage = GetSelectedText(LanguageComboBox);
        AppSettings.TimeZone = GetSelectedText(TimeZoneComboBox);
        AppSettings.Currency = GetSelectedText(CurrencyComboBox);
        AppSettings.ThemeName = GetSelectedText(ThemeComboBox);
        AppSettings.UseLightTheme = AppSettings.ThemeName == "Світла";

        MessageBox.Show("Налаштування збережено.", "CoffeeShop", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private static void SelectComboBoxItem(ComboBox comboBox, string value)
    {
        foreach (var item in comboBox.Items.OfType<ComboBoxItem>())
        {
            if ((item.Content?.ToString() ?? string.Empty) == value)
            {
                comboBox.SelectedItem = item;
                return;
            }
        }

        comboBox.SelectedIndex = 0;
    }

    private static string GetSelectedText(ComboBox comboBox)
    {
        return comboBox.SelectedItem is ComboBoxItem item
            ? item.Content?.ToString() ?? string.Empty
            : string.Empty;
    }
}
