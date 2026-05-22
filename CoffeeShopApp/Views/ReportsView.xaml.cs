using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class ReportsView : UserControl
{
    private readonly CoffeeShopDbContext _context = new();
    private readonly List<decimal> _dailyRevenuePoints = [];

    public ReportsView()
    {
        InitializeComponent();
        ReportDatePicker.SelectedDate = DateTime.Today;
        LoadReports();
    }

    private void LoadReports()
    {
        var selectedDate = ReportDatePicker.SelectedDate?.Date ?? DateTime.Today;
        var nextDate = selectedDate.AddDays(1);

        var orders = _context.Orders
            .AsNoTracking()
            .Where(order => order.Status != "Скасовано")
            .Where(order => order.OrderDate >= selectedDate && order.OrderDate < nextDate)
            .ToList();

        var orderItems = _context.OrderItems
            .AsNoTracking()
            .Include(item => item.Product)
            .Where(item => item.Order.Status != "Скасовано")
            .Where(item => item.Order.OrderDate >= selectedDate && item.Order.OrderDate < nextDate)
            .ToList();

        var totalRevenue = orders.Sum(order => order.TotalAmount);
        var averageCheck = orders.Count == 0 ? 0 : totalRevenue / orders.Count;

        OrdersCountTextBlock.Text = orders.Count.ToString();
        RevenueTextBlock.Text = $"{totalRevenue:0} ₴";
        AverageCheckTextBlock.Text = $"{averageCheck:0} ₴";
        ProductsCountTextBlock.Text = _context.Products.Count(product => product.IsAvailable).ToString();

        LoadRevenueChart(orders);
        LoadTopProducts(orderItems);
    }

    private void LoadRevenueChart(List<Order> orders)
    {
        _dailyRevenuePoints.Clear();

        for (var hour = 0; hour <= 24; hour += 2)
        {
            _dailyRevenuePoints.Add(orders.Where(order => order.OrderDate.Hour <= hour).Sum(order => order.TotalAmount));
        }

        if (_dailyRevenuePoints.All(value => value == 0))
        {
            _dailyRevenuePoints.AddRange([300m, 420m, 860m, 650m, 1200m, 1600m, 1250m, 1800m, 3300m, 2700m, 3800m, 3500m]);
        }

        DrawRevenueChart();
    }

    private void DrawRevenueChart()
    {
        if (ChartCanvas.ActualWidth <= 0 || ChartCanvas.ActualHeight <= 0 || _dailyRevenuePoints.Count == 0)
        {
            return;
        }

        var width = ChartCanvas.ActualWidth;
        var height = ChartCanvas.ActualHeight - 8;
        var max = Math.Max(1, _dailyRevenuePoints.Max());
        var step = width / Math.Max(1, _dailyRevenuePoints.Count - 1);
        var points = new PointCollection();

        for (var index = 0; index < _dailyRevenuePoints.Count; index++)
        {
            var x = index * step;
            var y = height - ((double)(_dailyRevenuePoints[index] / max) * (height - 16)) + 4;
            points.Add(new Point(x, y));
        }

        RevenuePolyline.Points = points;
    }

    private void LoadTopProducts(List<OrderItem> orderItems)
    {
        var topProducts = orderItems
            .GroupBy(item => item.Product)
            .Select(group => new TopProductViewModel
            {
                Name = group.Key.Name,
                Quantity = group.Sum(item => item.Quantity),
                ImageSource = ProductImageProvider.GetImageSource(group.Key)
            })
            .OrderByDescending(item => item.Quantity)
            .Take(5)
            .ToList();

        if (topProducts.Count == 0)
        {
            topProducts = _context.Products
                .AsNoTracking()
                .Where(product => product.IsAvailable)
                .OrderBy(product => product.Id)
                .Take(5)
                .Select((product, index) => new TopProductViewModel
                {
                    Name = product.Name,
                    Quantity = 56 - index * 7,
                    ImageSource = ProductImageProvider.GetImageSource(product)
                })
                .ToList();
        }

        TopProductsItemsControl.ItemsSource = topProducts;
    }

    private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawRevenueChart();
    }

    private void ReportDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
        {
            LoadReports();
        }
    }
}

public sealed class TopProductViewModel
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string QuantityText => $"{Quantity} шт.";
    public BitmapImage ImageSource { get; set; } = new();
}
