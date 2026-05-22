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
        UpdateCurrentUserText();

        if (CartList is not null)
        {
            CartList.ItemsSource = _cart;
        }

        StartClock();
        LoadProducts();
        RefreshCart();
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
        var now = DateTime.Now;

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
            TotalText.Text = $"{total:0} грн";
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

        var order = new Order
        {
            OrderDate = DateTime.Now,
            TotalAmount = _cart.Sum(item => item.LineTotal),
            Status = "Виконано"
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
