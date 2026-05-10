using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class MainWindow : Window
{
    private readonly CoffeeShopDbContext _context = new();
    private readonly List<Product> _products = new();
    private readonly ObservableCollection<CartItemViewModel> _cart = new();

    private int _selectedCategoryId;
    private string _searchText = string.Empty;

    private ListBox? CartList => FindName("CartListBox") as ListBox;
    private ItemsControl? ProductsControl => FindName("ProductsItemsControl") as ItemsControl;
    private TextBlock? TotalText => FindName("TotalTextBlock") as TextBlock;
    private TextBlock? ItemsCountText => FindName("ItemsCountTextBlock") as TextBlock;

    public MainWindow()
    {
        InitializeComponent();

        if (CartList is not null)
        {
            CartList.ItemsSource = _cart;
        }

        LoadProducts();
        RefreshCart();
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
            TotalAmount = _cart.Sum(item => item.LineTotal)
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

        MessageBox.Show("Замовлення збережено!", "CoffeeShop", MessageBoxButton.OK, MessageBoxImage.Information);

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

    private void ProductsButton_Click(object sender, RoutedEventArgs e)
    {
        new ProductsWindow().ShowDialog();
        LoadProducts();
    }

    private void CategoriesButton_Click(object sender, RoutedEventArgs e)
    {
        new CategoriesWindow().ShowDialog();
        LoadProducts();
    }

    private void OrdersHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        new OrdersHistoryWindow().ShowDialog();
    }

    private void ReportsButton_Click(object sender, RoutedEventArgs e)
    {
        new ReportsWindow().ShowDialog();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().ShowDialog();
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
        PhotoBrush = ProductVisuals.GetBrush(product.CategoryId);
        AccentBrush = product.Id == 9
            ? new SolidColorBrush(Color.FromRgb(154, 100, 45))
            : new SolidColorBrush(Color.FromRgb(231, 222, 210));
        AccentThickness = product.Id == 9 ? new Thickness(1.5) : new Thickness(1);
    }

    public Product Product { get; }
    public Brush PhotoBrush { get; }
    public Brush AccentBrush { get; }
    public Thickness AccentThickness { get; }
}

public sealed class CartItemViewModel
{
    public CartItemViewModel(Product product)
    {
        Product = product;
        PhotoBrush = ProductVisuals.GetBrush(product.CategoryId);
    }

    public Product Product { get; }
    public int Quantity { get; set; } = 1;
    public decimal LineTotal => Product.Price * Quantity;
    public Brush PhotoBrush { get; }
}

public static class ProductVisuals
{
    public static Brush GetBrush(int categoryId)
    {
        var brush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1)
        };

        switch (categoryId)
        {
            case 1:
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(220, 188, 141), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(105, 66, 33), 1));
                break;
            case 2:
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(238, 218, 154), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(184, 93, 31), 1));
                break;
            case 3:
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(242, 224, 186), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(157, 105, 54), 1));
                break;
            case 4:
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(238, 213, 160), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(83, 109, 74), 1));
                break;
            case 5:
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(244, 208, 147), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(164, 86, 28), 1));
                break;
            case 6:
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(245, 218, 214), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(132, 42, 48), 1));
                break;
            default:
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(230, 218, 205), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(92, 72, 51), 1));
                break;
        }

        return brush;
    }
}
