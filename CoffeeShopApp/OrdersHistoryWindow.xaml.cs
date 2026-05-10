using System.Windows;
using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class OrdersHistoryWindow : Window
{
    private readonly CoffeeShopDbContext _context = new();

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
            .OrderByDescending(order => order.OrderDate)
            .ToList();

        OrdersDataGrid.ItemsSource = orders
            .Select(order => new
            {
                order.Id,
                Date = order.OrderDate.ToString("dd.MM.yyyy HH:mm"),
                ItemsCount = order.Items.Sum(item => item.Quantity),
                Products = string.Join(", ", order.Items.Select(item => item.Product.Name)),
                Total = order.TotalAmount
            })
            .ToList();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadOrders();

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
