using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class MainWindow : Window
{
    private readonly CoffeeShopDbContext _context = new();
    private readonly List<Product> _products = new();
    private readonly ObservableCollection<CartItemViewModel> _cart = new();
    private readonly DispatcherTimer _clockTimer = new();

    private int _selectedCategoryId;
    private string _searchText = string.Empty;
    private ProductFilterMode _productFilterMode = ProductFilterMode.All;

    private ListBox? CartList => FindName("CartListBox") as ListBox;
    private ItemsControl? ProductsControl => FindName("ProductsItemsControl") as ItemsControl;
    private TextBlock? TotalText => FindName("TotalTextBlock") as TextBlock;
    private TextBlock? ItemsCountText => FindName("ItemsCountTextBlock") as TextBlock;
    private TextBlock? TodayText => FindName("TodayTextBlock") as TextBlock;
    private TextBlock? TimeText => FindName("TimeTextBlock") as TextBlock;
    private TextBlock? FooterUserText => FindName("FooterUserTextBlock") as TextBlock;
    private TextBlock? SearchPlaceholderText => FindName("SearchPlaceholderTextBlock") as TextBlock;
    private Grid? OrdersPage => FindName("OrdersPageGrid") as Grid;
    private Border? Cart => FindName("CartPanel") as Border;
    private ContentControl? PageContent => FindName("PageContentControl") as ContentControl;

    public MainWindow()
    {
        InitializeComponent();
        Title = $"CoffeeShop - {App.CurrentUserName}";
        ApplyLanguage();
        UpdateCurrentUserText();

        if (CartList is not null)
        {
            CartList.ItemsSource = _cart;
        }

        StartClock();
        AppSettings.Changed += AppSettings_Changed;
        ApplyTheme();
        LoadProducts();
        RefreshCart();
    }

    private void AppSettings_Changed(object? sender, EventArgs e)
    {
        ApplyTheme();
        ApplyLanguage();
        UpdateClock();
        UpdateCurrentUserText();
        ApplyFilters();
        RefreshCart();

        if (PageContent?.Content is ReportsView)
        {
            PageContent.Content = new ReportsView();
        }

        if (PageContent?.Content is OrdersHistoryView)
        {
            PageContent.Content = new OrdersHistoryView();
        }

        if (PageContent?.Content is SettingsView)
        {
            PageContent.Content = new SettingsView();
        }

        if (PageContent?.Content is MenuView)
        {
            PageContent.Content = new MenuView();
        }

        if (PageContent?.Content is CategoriesView)
        {
            PageContent.Content = new CategoriesView();
        }

        ApplyTheme();
    }

    private void StartClock()
    {
        UpdateClock();
        _clockTimer.Interval = TimeSpan.FromSeconds(1);
        _clockTimer.Tick += (_, _) => UpdateClock();
        _clockTimer.Start();
    }

    private void UpdateCurrentUserText()
    {
        if (FooterUserText is not null)
        {
            FooterUserText.Text = $"Користувач: {App.CurrentUserName}";
        }
    }

    private void UpdateClock()
    {
        var now = AppSettings.GetCurrentTime();

        if (TodayText is not null)
        {
            TodayText.Text = now.ToString("d MMMM yyyy", new CultureInfo("uk-UA"));
        }

        if (TimeText is not null)
        {
            TimeText.Text = now.ToString("HH:mm");
        }
    }

    private void LoadProducts()
    {
        _products.Clear();
        _products.AddRange(_context.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .ToList());

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var products = _products.AsEnumerable();

        if (_selectedCategoryId > 0)
        {
            products = products.Where(product => product.CategoryId == _selectedCategoryId);
        }

        if (!string.IsNullOrWhiteSpace(_searchText))
        {
            products = products.Where(product =>
                product.Name.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase) ||
                product.Description.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase));
        }

        products = _productFilterMode switch
        {
            ProductFilterMode.Under50 => products.Where(product => product.Price <= 50m),
            ProductFilterMode.From50To80 => products.Where(product => product.Price > 50m && product.Price <= 80m),
            ProductFilterMode.From80 => products.Where(product => product.Price > 80m),
            _ => products
        };

        if (ProductsControl is not null)
        {
            ProductsControl.ItemsSource = products
                .Select(product => new ProductCardViewModel(product))
                .ToList();
        }
    }

    private void AddProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Product product })
        {
            AddProductToCart(product);
        }
    }

    public void AddToCartButton_Click(object sender, RoutedEventArgs e)
    {
        if (FindName("ProductsListBox") is ListBox { SelectedItem: Product product })
        {
            AddProductToCart(product);
        }
    }

    private void AddProductToCart(Product product)
    {
        var existingItem = _cart.FirstOrDefault(item => item.Product.Id == product.Id);
        if (existingItem is null)
        {
            _cart.Add(new CartItemViewModel(product));
        }
        else
        {
            existingItem.Quantity++;
        }

        RefreshCart();
    }

    private void IncreaseCartButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: int productId })
        {
            var item = _cart.FirstOrDefault(cartItem => cartItem.Product.Id == productId);
            if (item is not null)
            {
                item.Quantity++;
                RefreshCart();
            }
        }
    }

    private void DecreaseCartButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: int productId })
        {
            var item = _cart.FirstOrDefault(cartItem => cartItem.Product.Id == productId);
            if (item is null)
            {
                return;
            }

            item.Quantity--;
            if (item.Quantity <= 0)
            {
                _cart.Remove(item);
            }

            RefreshCart();
        }
    }

    private void RefreshCart()
    {
        CartList?.Items.Refresh();

        var total = _cart.Sum(item => item.LineTotal);
        var count = _cart.Sum(item => item.Quantity);

        if (TotalText is not null)
        {
            TotalText.Text = AppSettings.FormatMoney(total);
        }

        if (ItemsCountText is not null)
        {
            ItemsCountText.Text = $"Кількість позицій: {count}";
        }
    }

    private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
    {
        if (_cart.Count == 0)
        {
            MessageBox.Show("Замовлення порожнє.", "CoffeeShop", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var paymentWindow = new PaymentMethodWindow { Owner = this };
        if (paymentWindow.ShowDialog() != true)
        {
            return;
        }

        var order = new Order
        {
            OrderDate = DateTime.Now,
            TotalAmount = _cart.Sum(item => item.LineTotal),
            Status = "Виконано",
            PaymentMethod = paymentWindow.PaymentMethod
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        foreach (var item in _cart)
        {
            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.Product.Id,
                Quantity = item.Quantity,
                Price = item.Product.Price
            });
        }

        _context.SaveChanges();

        if (AppSettings.ShowOrderConfirmation)
        {
            MessageBox.Show("Замовлення збережено!", "CoffeeShop", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        _cart.Clear();
        RefreshCart();
    }

    private void ClearCartButton_Click(object sender, RoutedEventArgs e)
    {
        _cart.Clear();
        RefreshCart();
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string tag } button && int.TryParse(tag, out var categoryId))
        {
            _selectedCategoryId = categoryId;
            SetActiveCategoryButton(button);
            ApplyFilters();
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            _searchText = textBox.Text.Trim();
            if (SearchPlaceholderText is not null)
            {
                SearchPlaceholderText.Visibility = string.IsNullOrWhiteSpace(textBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        ApplyFilters();
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.ContextMenu is not null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void FilterMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string filter })
        {
            return;
        }

        _productFilterMode = filter switch
        {
            "Under50" => ProductFilterMode.Under50,
            "From50To80" => ProductFilterMode.From50To80,
            "From80" => ProductFilterMode.From80,
            _ => ProductFilterMode.All
        };

        if (FilterButton is not null)
        {
            FilterButton.Content = GetFilterButtonText();
        }

        ApplyFilters();
    }

    private void SetActiveCategoryButton(Button activeButton)
    {
        if (activeButton.Parent is not Panel panel)
        {
            return;
        }

        foreach (var button in panel.Children.OfType<Button>())
        {
            button.Background = new SolidColorBrush(Color.FromRgb(242, 238, 232));
            button.Foreground = new SolidColorBrush(Color.FromRgb(59, 44, 29));
        }

        activeButton.Background = new SolidColorBrush(Color.FromRgb(59, 44, 29));
        activeButton.Foreground = Brushes.White;
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            SetActiveNavButton(button);
        }

        ShowOrdersPage();
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            SetActiveNavButton(button);
        }

        ShowEmbeddedPage(new MenuView());
    }

    private void SetActiveNavButton(Button activeButton)
    {
        if (activeButton.Parent is not Panel panel)
        {
            return;
        }

        foreach (var button in panel.Children.OfType<Button>())
        {
            button.Background = Brushes.Transparent;
            button.Foreground = Brushes.White;
        }

        activeButton.Background = new SolidColorBrush(Color.FromRgb(185, 155, 119));
        activeButton.Foreground = Brushes.White;
    }

    private void CategoriesButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            SetActiveNavButton(button);
        }

        ShowEmbeddedPage(new CategoriesView());
    }

    private void OrdersHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            SetActiveNavButton(button);
        }

        ShowEmbeddedPage(new OrdersHistoryView());
    }

    private void ReportsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            SetActiveNavButton(button);
        }

        ShowEmbeddedPage(new ReportsView());
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            SetActiveNavButton(button);
        }

        ShowEmbeddedPage(new SettingsView());
    }

    private void ShowOrdersPage()
    {
        if (OrdersPage is not null)
        {
            OrdersPage.Visibility = Visibility.Visible;
        }

        if (Cart is not null)
        {
            Cart.Visibility = Visibility.Visible;
        }

        if (PageContent is not null)
        {
            PageContent.Content = null;
            PageContent.Visibility = Visibility.Collapsed;
        }

        LoadProducts();
        ApplyTheme();
    }

    private void ShowEmbeddedPage(UserControl page)
    {
        if (OrdersPage is not null)
        {
            OrdersPage.Visibility = Visibility.Collapsed;
        }

        if (Cart is not null)
        {
            Cart.Visibility = Visibility.Collapsed;
        }

        if (PageContent is not null)
        {
            PageContent.Content = page;
            PageContent.Visibility = Visibility.Visible;
        }

        Dispatcher.BeginInvoke(ApplyTheme);
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleWindowState();
            return;
        }

        DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleWindowState();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ToggleWindowState()
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void ApplyTheme()
    {
        Background = AppSettings.UseLightTheme
            ? new SolidColorBrush(Color.FromRgb(248, 246, 242))
            : new SolidColorBrush(Color.FromRgb(48, 39, 27));

        ApplyThemeToElement(OrdersPage);
        ApplyThemeToElement(Cart);
        ApplyThemeToElement(PageContent);
    }

    private void ApplyThemeToElement(DependencyObject? element)
    {
        if (element is null)
        {
            return;
        }

        if (element is Border border)
        {
            ApplyBorderTheme(border);
        }

        if (element is TextBlock textBlock)
        {
            ApplyTextTheme(textBlock);
        }

        if (element is TextBox textBox)
        {
            textBox.Background = AppSettings.UseLightTheme
                ? Brushes.White
                : new SolidColorBrush(Color.FromRgb(58, 45, 32));
            textBox.Foreground = AppSettings.UseLightTheme
                ? new SolidColorBrush(Color.FromRgb(23, 18, 14))
                : new SolidColorBrush(Color.FromRgb(248, 246, 242));
            textBox.BorderBrush = AppSettings.UseLightTheme
                ? new SolidColorBrush(Color.FromRgb(231, 222, 210))
                : new SolidColorBrush(Color.FromRgb(91, 74, 54));
        }

        if (element is DataGrid dataGrid)
        {
            dataGrid.Background = AppSettings.UseLightTheme
                ? Brushes.White
                : new SolidColorBrush(Color.FromRgb(58, 45, 32));
            dataGrid.Foreground = AppSettings.UseLightTheme
                ? new SolidColorBrush(Color.FromRgb(23, 18, 14))
                : new SolidColorBrush(Color.FromRgb(248, 246, 242));
            dataGrid.RowBackground = dataGrid.Background;
            dataGrid.AlternatingRowBackground = AppSettings.UseLightTheme
                ? new SolidColorBrush(Color.FromRgb(251, 248, 244))
                : new SolidColorBrush(Color.FromRgb(66, 52, 37));
        }

        var childrenCount = VisualTreeHelper.GetChildrenCount(element);
        for (var index = 0; index < childrenCount; index++)
        {
            ApplyThemeToElement(VisualTreeHelper.GetChild(element, index));
        }
    }

    private static void ApplyBorderTheme(Border border)
    {
        if (AppSettings.UseLightTheme)
        {
            if (IsBrushColor(border.Background, 58, 45, 32) ||
                IsBrushColor(border.Background, 66, 52, 37) ||
                IsBrushColor(border.Background, 48, 39, 27))
            {
                border.Background = Brushes.White;
            }

            if (IsBrushColor(border.BorderBrush, 91, 74, 54))
            {
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 222, 210));
            }

            return;
        }

        if (IsLightBrush(border.Background))
        {
            border.Background = new SolidColorBrush(Color.FromRgb(58, 45, 32));
        }

        if (IsLightBrush(border.BorderBrush))
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(91, 74, 54));
        }
    }

    private static void ApplyTextTheme(TextBlock textBlock)
    {
        if (IsGreenOrRedBrush(textBlock.Foreground))
        {
            return;
        }

        if (AppSettings.UseLightTheme)
        {
            if (IsBrushColor(textBlock.Foreground, 248, 246, 242) ||
                IsBrushColor(textBlock.Foreground, 216, 199, 179))
            {
                textBlock.Foreground = IsStrongText(textBlock)
                    ? new SolidColorBrush(Color.FromRgb(23, 18, 14))
                    : new SolidColorBrush(Color.FromRgb(95, 88, 80));
            }

            return;
        }

        textBlock.Foreground = IsStrongText(textBlock)
            ? new SolidColorBrush(Color.FromRgb(248, 246, 242))
            : new SolidColorBrush(Color.FromRgb(216, 199, 179));
    }

    private static bool IsStrongText(TextBlock textBlock)
    {
        return textBlock.FontWeight.ToOpenTypeWeight() >= FontWeights.SemiBold.ToOpenTypeWeight() ||
               textBlock.FontSize >= 18;
    }

    private static bool IsLightBrush(Brush? brush)
    {
        return brush is SolidColorBrush solidColorBrush &&
               solidColorBrush.Color.R > 210 &&
               solidColorBrush.Color.G > 200 &&
               solidColorBrush.Color.B > 185;
    }

    private static bool IsGreenOrRedBrush(Brush? brush)
    {
        if (brush is not SolidColorBrush solidColorBrush)
        {
            return false;
        }

        var color = solidColorBrush.Color;
        return color.G > color.R + 25 || color.R > color.G + 45;
    }

    private static bool IsBrushColor(Brush? brush, byte red, byte green, byte blue)
    {
        return brush is SolidColorBrush solidColorBrush &&
               solidColorBrush.Color.R == red &&
               solidColorBrush.Color.G == green &&
               solidColorBrush.Color.B == blue;
    }

    private void ApplyLanguage()
    {
        if (FindName("OrdersNavButton") is Button ordersButton)
        {
            ordersButton.Content = "Замовлення";
        }

        if (FindName("MenuNavButton") is Button menuButton)
        {
            menuButton.Content = "Меню";
        }

        if (FindName("CategoriesNavButton") is Button categoriesButton)
        {
            categoriesButton.Content = "Категорії";
        }

        if (FindName("HistoryNavButton") is Button historyButton)
        {
            historyButton.Content = "Історія замовлень";
        }

        if (FindName("ReportsNavButton") is Button reportsButton)
        {
            reportsButton.Content = "Звіти";
        }

        if (FindName("SettingsNavButton") is Button settingsButton)
        {
            settingsButton.Content = "Налаштування";
        }

        SetText("TodayLabelTextBlock", "Сьогодні");
        SetText("TimeLabelTextBlock", "Час");
        SetText("OrdersTitleTextBlock", "Меню");
        SetText("OrdersSubtitleTextBlock", "Оберіть товари для замовлення");
        SetText("SearchPlaceholderTextBlock", "Пошук товарів...");
        SetText("HotDrinksFirstLineTextBlock", "Гарячі");
        SetText("HotDrinksSecondLineTextBlock", "напої");
        SetText("ColdDrinksFirstLineTextBlock", "Холодні");
        SetText("ColdDrinksSecondLineTextBlock", "напої");
        SetText("CurrentOrderTitleTextBlock", "Поточне замовлення");
        SetText("TotalLabelTextBlock", "Разом:");
        SetText("DatabaseStatusTextBlock", "●  Підключено до бази даних");

        SetButton("AllCategoryButton", "Усі");
        SetButton("CoffeeCategoryButton", "Кава");
        SetButton("SnacksCategoryButton", "Перекуси");
        SetButton("BakeryCategoryButton", "Випічка");
        SetButton("DessertsCategoryButton", "Десерти");
        SetButton("SaveOrderButton", "Оформити замовлення");
        SetButton("ClearOrderButton", "Очистити замовлення");
        SetButton("FilterButton", GetFilterButtonText());

        UpdateFilterMenuLanguage();
    }

    private void UpdateFilterMenuLanguage()
    {
        if (FilterButton?.ContextMenu is not { } contextMenu)
        {
            return;
        }

        foreach (var item in contextMenu.Items.OfType<MenuItem>())
        {
            item.Header = item.Tag switch
            {
                "All" => "Без фільтра",
                "Under50" => "До 50 грн",
                "From50To80" => "50-80 грн",
                "From80" => "Від 80 грн",
                _ => item.Header
            };
        }
    }

    private string GetFilterButtonText()
    {
        return _productFilterMode switch
        {
            ProductFilterMode.Under50 => "До 50 грн",
            ProductFilterMode.From50To80 => "50-80 грн",
            ProductFilterMode.From80 => "Від 80 грн",
            _ => "Фільтр"
        };
    }

    private void SetText(string name, string value)
    {
        if (FindName(name) is TextBlock textBlock)
        {
            textBlock.Text = value;
        }
    }

    private void SetButton(string name, string value)
    {
        if (FindName(name) is Button button)
        {
            button.Content = value;
        }
    }
}

public enum ProductFilterMode
{
    All,
    Under50,
    From50To80,
    From80
}

public sealed class ProductCardViewModel
{
    public ProductCardViewModel(Product product)
    {
        Product = product;
        ImageSource = ProductImageProvider.GetImageSource(product);
        AccentBrush = new SolidColorBrush(Color.FromRgb(231, 222, 210));
        AccentThickness = new Thickness(1);
    }

    public Product Product { get; }
    public BitmapImage ImageSource { get; }
    public Brush AccentBrush { get; }
    public Thickness AccentThickness { get; }
    public string PriceText => AppSettings.FormatMoney(Product.Price);
}

public sealed class CartItemViewModel
{
    public CartItemViewModel(Product product)
    {
        Product = product;
        ImageSource = ProductImageProvider.GetImageSource(product);
    }

    public Product Product { get; }
    public BitmapImage ImageSource { get; }
    public int Quantity { get; set; } = 1;
    public decimal LineTotal => Product.Price * Quantity;
    public string PriceText => AppSettings.FormatMoney(Product.Price);
    public string LineTotalText => AppSettings.FormatMoney(LineTotal);
}

public static class ProductImageProvider
{
    public static BitmapImage GetImageSource(Product product)
    {
        var productImageName = GetProductImageName(product);
        var imagePaths = new[]
        {
            product.ImagePath,
            string.IsNullOrWhiteSpace(product.ImagePath) || product.ImagePath.Contains('/') || product.ImagePath.Contains('\\')
                ? string.Empty
                : $"Assets/Products/{product.ImagePath}",
            $"Assets/Products/{product.Name}.png",
            $"Assets/Products/{productImageName}.png",
            "Assets/Products/Еспресо.png"
        };

        foreach (var imagePath in imagePaths.Where(path => !string.IsNullOrWhiteSpace(path)).Distinct())
        {
            var image = LoadImage(imagePath);
            if (image is not null)
            {
                return image;
            }
        }

        return new BitmapImage();
    }

    private static string GetProductImageName(Product product)
    {
        return product.Id switch
        {
            6 => "Флет вайт",
            7 => "Раф",
            10 => "Еспресо тонік",
            14 => "Чорний чай",
            24 => "Сінабон",
            32 => "Айс матча лате",
            _ => product.Name
        };
    }

    private static BitmapImage? LoadImage(string imagePath)
    {
        try
        {
            var cleanPath = imagePath.Replace("\\", "/").TrimStart('/');
            var fileImage = LoadFileImage(imagePath, cleanPath);
            if (fileImage is not null)
            {
                return fileImage;
            }

            var uri = new Uri($"pack://application:,,,/{cleanPath}", UriKind.Absolute);
            return CreateBitmapImage(uri);
        }
        catch
        {
            return null;
        }
    }

    private static BitmapImage? LoadFileImage(string originalPath, string cleanPath)
    {
        var possiblePaths = new List<string>();

        if (Path.IsPathRooted(originalPath))
        {
            possiblePaths.Add(originalPath);
        }

        var projectDirectory = FindProjectDirectory();
        if (projectDirectory is not null)
        {
            possiblePaths.Add(Path.Combine(projectDirectory, cleanPath.Replace("/", Path.DirectorySeparatorChar.ToString())));
        }

        possiblePaths.Add(Path.Combine(AppContext.BaseDirectory, cleanPath.Replace("/", Path.DirectorySeparatorChar.ToString())));

        foreach (var possiblePath in possiblePaths.Distinct())
        {
            if (!File.Exists(possiblePath))
            {
                continue;
            }

            return CreateBitmapImage(new Uri(possiblePath, UriKind.Absolute));
        }

        return null;
    }

    private static string? FindProjectDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CoffeeShopApp.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static BitmapImage CreateBitmapImage(Uri uri)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.UriSource = uri;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        image.Freeze();

        return image;
    }
}
