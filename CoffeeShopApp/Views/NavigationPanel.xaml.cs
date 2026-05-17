using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CoffeeShopApp;

public partial class NavigationPanel : UserControl
{
    public static readonly DependencyProperty ActivePageProperty =
        DependencyProperty.Register(
            nameof(ActivePage),
            typeof(string),
            typeof(NavigationPanel),
            new PropertyMetadata(string.Empty, OnActivePageChanged));

    public NavigationPanel()
    {
        InitializeComponent();
        UpdateActiveButton();
    }

    public string ActivePage
    {
        get => (string)GetValue(ActivePageProperty);
        set => SetValue(ActivePageProperty, value);
    }

    private static void OnActivePageChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is NavigationPanel navigationPanel)
        {
            navigationPanel.UpdateActiveButton();
        }
    }

    private void OrdersButton_Click(object sender, RoutedEventArgs e)
    {
        CloseCurrentWindow();
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo("Menu", new MenuWindow());
    }

    private void CategoriesButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo("Categories", new CategoriesWindow());
    }

    private void OrdersHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo("OrdersHistory", new OrdersHistoryWindow());
    }

    private void ReportsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo("Reports", new ReportsWindow());
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo("Settings", new SettingsWindow());
    }

    private void NavigateTo(string page, Window window)
    {
        if (ActivePage == page)
        {
            return;
        }

        var currentWindow = Window.GetWindow(this);
        window.Owner = currentWindow?.Owner;
        currentWindow?.Hide();
        window.ShowDialog();
        currentWindow?.Close();
    }

    private void CloseCurrentWindow()
    {
        Window.GetWindow(this)?.Close();
    }

    private void UpdateActiveButton()
    {
        if (!IsInitialized)
        {
            return;
        }

        var buttons = new Dictionary<string, Button>
        {
            ["Orders"] = OrdersButton,
            ["Menu"] = MenuButton,
            ["Categories"] = CategoriesButton,
            ["OrdersHistory"] = OrdersHistoryButton,
            ["Reports"] = ReportsButton,
            ["Settings"] = SettingsButton
        };

        foreach (var button in buttons.Values)
        {
            button.Background = Brushes.Transparent;
        }

        if (buttons.TryGetValue(ActivePage, out var activeButton))
        {
            activeButton.Background = new SolidColorBrush(Color.FromRgb(185, 155, 119));
        }
    }
}
