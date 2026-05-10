using System.Windows;
using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class ReportsWindow : Window
{
    private readonly CoffeeShopDbContext _context = new();

    public ReportsWindow()
    {
        InitializeComponent();
        LoadReports();
    }

    private void LoadReports()
    {
        var orders = _context.Orders.AsNoTracking().ToList();

        OrdersCountTextBlock.Text = orders.Count.ToString();
        RevenueTextBlock.Text = $"{orders.Sum(order => order.TotalAmount):0} ₴";
        ProductsCountTextBlock.Text = _context.Products.Count().ToString();

        var orderItems = _context.OrderItems
            .AsNoTracking()
            .Include(item => item.Product)
            .ThenInclude(product => product.Category)
            .ToList();

        CategorySalesDataGrid.ItemsSource = orderItems
            .GroupBy(item => item.Product.Category.Name)
            .Select(group => new
            {
                Category = group.Key,
                Quantity = group.Sum(item => item.Quantity),
                Total = group.Sum(item => item.Quantity * item.Price)
            })
            .OrderByDescending(row => row.Total)
            .ToList();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
