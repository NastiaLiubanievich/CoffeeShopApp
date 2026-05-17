using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class OrdersHistoryWindow : Window
{
    private const string CompletedStatus = "Виконано";
    private const string CanceledStatus = "Скасовано";

    private readonly CoffeeShopDbContext _context = new();
    private string _selectedStatus = CompletedStatus;

    public OrdersHistoryWindow()
    {
        InitializeComponent();
        LoadOrders();
    }

    private void LoadOrders()
    {
        var orders = _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .Where(order => order.Status == _selectedStatus)
            .OrderByDescending(order => order.OrderDate)
            .ToList();

        OrdersItemsControl.ItemsSource = orders
            .Select(order => new OrderHistoryRowViewModel
            {
                Id = order.Id,
                Date = order.OrderDate.ToString("dd.MM.yyyy HH:mm"),
                Products = order.Items.Count == 0
                    ? "Без позицій"
                    : string.Join(", ", order.Items.Select(item => $"{item.Product.Name} x{item.Quantity}")),
                Total = order.TotalAmount,
                Status = order.Status
            })
            .ToList();
    }

    private void CompletedFilterButton_Click(object sender, RoutedEventArgs e)
    {
        _selectedStatus = CompletedStatus;
        SetActiveFilter();
        LoadOrders();
    }

    private void CanceledFilterButton_Click(object sender, RoutedEventArgs e)
    {
        _selectedStatus = CanceledStatus;
        SetActiveFilter();
        LoadOrders();
    }

    private void ViewOrderButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: int orderId })
        {
            return;
        }

        var order = _context.Orders
            .AsNoTracking()
            .Include(item => item.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefault(item => item.Id == orderId);

        if (order is null)
        {
            MessageBox.Show("Замовлення не знайдено.", "Історія замовлень", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var products = order.Items.Count == 0
            ? "Товари не вказані"
            : string.Join("\n", order.Items.Select(item => $"{item.Product.Name} x{item.Quantity} - {item.Price * item.Quantity:0} грн"));

        MessageBox.Show(
            $"Замовлення #{order.Id:0000}\n" +
            $"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}\n" +
            $"Статус: {order.Status}\n\n" +
            $"{products}\n\n" +
            $"Сума: {order.TotalAmount:0} грн",
            "Деталі замовлення",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void CancelOrderButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: int orderId })
        {
            return;
        }

        var result = MessageBox.Show(
            $"Скасувати замовлення #{orderId:0000}?",
            "Скасування замовлення",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        var order = _context.Orders.FirstOrDefault(item => item.Id == orderId);
        if (order is null)
        {
            MessageBox.Show("Замовлення не знайдено.", "Історія замовлень", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        order.Status = CanceledStatus;
        _context.SaveChanges();
        LoadOrders();
    }

    private void SetActiveFilter()
    {
        SetFilterButtonStyle(CompletedFilterButton, _selectedStatus == CompletedStatus);
        SetFilterButtonStyle(CanceledFilterButton, _selectedStatus == CanceledStatus);
    }

    private static void SetFilterButtonStyle(Button button, bool isActive)
    {
        button.Background = isActive
            ? new SolidColorBrush(Color.FromRgb(59, 44, 29))
            : new SolidColorBrush(Color.FromRgb(242, 238, 232));
        button.Foreground = isActive ? Brushes.White : new SolidColorBrush(Color.FromRgb(59, 44, 29));
    }
}

public sealed class OrderHistoryRowViewModel
{
    public int Id { get; set; }
    public string OrderNumber => $"#{Id:0000}";
    public string Date { get; set; } = string.Empty;
    public string Products { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string TotalText => $"{Total:0} грн";
    public string Status { get; set; } = string.Empty;
    public bool CanCancel => Status == "Виконано";
    public Brush StatusBackground => Status == "Скасовано"
        ? new SolidColorBrush(Color.FromRgb(255, 235, 232))
        : new SolidColorBrush(Color.FromRgb(232, 244, 231));
    public Brush StatusForeground => Status == "Скасовано"
        ? new SolidColorBrush(Color.FromRgb(192, 62, 52))
        : new SolidColorBrush(Color.FromRgb(63, 139, 69));
}
