using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class ReportsView : UserControl
{
    private const string CanceledStatus = "Скасовано";
    private readonly CoffeeShopDbContext _context = new();
    private readonly List<decimal> _revenuePoints = [];

    public ReportsView()
    {
        InitializeComponent();
        ApplyLanguage();
        ReportDatePicker.SelectedDate = DateTime.Today;
        LoadReports();
    }

    private bool IsAllTimeSelected => PeriodComboBox.SelectedIndex == 1;

    private void LoadReports()
    {
        var ordersQuery = _context.Orders
            .AsNoTracking()
            .Where(order => order.Status != CanceledStatus);

        var itemsQuery = _context.OrderItems
            .AsNoTracking()
            .Include(item => item.Order)
            .Include(item => item.Product)
            .Where(item => item.Order.Status != CanceledStatus);

        if (!IsAllTimeSelected)
        {
            var selectedDate = ReportDatePicker.SelectedDate?.Date ?? DateTime.Today;
            var nextDate = selectedDate.AddDays(1);

            ordersQuery = ordersQuery.Where(order => order.OrderDate >= selectedDate && order.OrderDate < nextDate);
            itemsQuery = itemsQuery.Where(item => item.Order.OrderDate >= selectedDate && item.Order.OrderDate < nextDate);
        }

        var orders = ordersQuery.OrderBy(order => order.OrderDate).ToList();
        var orderItems = itemsQuery.ToList();
        var totalRevenue = orders.Sum(order => order.TotalAmount);
        var averageCheck = orders.Count == 0 ? 0 : totalRevenue / orders.Count;

        OrdersCountTextBlock.Text = orders.Count.ToString();
        RevenueTextBlock.Text = AppSettings.FormatMoney(totalRevenue);
        AverageCheckTextBlock.Text = AppSettings.FormatMoney(averageCheck);
        ProductsCountTextBlock.Text = _context.Products.Count(product => product.IsAvailable).ToString();
        ChartTitleTextBlock.Text = GetChartTitle();
        ReportDatePicker.IsEnabled = !IsAllTimeSelected;

        LoadRevenueChart(orders);
        LoadTopProducts(orderItems);
    }

    private void LoadRevenueChart(List<Order> orders)
    {
        _revenuePoints.Clear();

        if (IsAllTimeSelected)
        {
            LoadAllTimeRevenuePoints(orders);
        }
        else
        {
            LoadDailyRevenuePoints(orders);
        }

        DrawRevenueChart();
    }

    private void LoadDailyRevenuePoints(List<Order> orders)
    {
        for (var hour = 0; hour <= 24; hour += 2)
        {
            _revenuePoints.Add(orders
                .Where(order => order.OrderDate.Hour <= hour)
                .Sum(order => order.TotalAmount));
        }
    }

    private void LoadAllTimeRevenuePoints(List<Order> orders)
    {
        var dailyTotals = orders
            .GroupBy(order => order.OrderDate.Date)
            .OrderBy(group => group.Key)
            .Select(group => group.Sum(order => order.TotalAmount))
            .ToList();

        if (dailyTotals.Count == 0)
        {
            _revenuePoints.Add(0);
            return;
        }

        decimal runningTotal = 0;
        foreach (var total in dailyTotals)
        {
            runningTotal += total;
            _revenuePoints.Add(runningTotal);
        }
    }

    private void DrawRevenueChart()
    {
        if (ChartCanvas.ActualWidth <= 0 || ChartCanvas.ActualHeight <= 0 || _revenuePoints.Count == 0)
        {
            return;
        }

        var width = ChartCanvas.ActualWidth;
        var height = ChartCanvas.ActualHeight - 8;
        var max = Math.Max(1, _revenuePoints.Max());
        var step = width / Math.Max(1, _revenuePoints.Count - 1);
        var points = new PointCollection();

        for (var index = 0; index < _revenuePoints.Count; index++)
        {
            var x = index * step;
            var y = height - ((double)(_revenuePoints[index] / max) * (height - 16)) + 4;
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

        TopProductsItemsControl.ItemsSource = topProducts;
        EmptyTopProductsTextBlock.Visibility = topProducts.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
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

    private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
        {
            LoadReports();
        }
    }

    private void ApplyLanguage()
    {
        var english = AppSettings.IsEnglish;

        TitleTextBlock.Text = english ? "Reports" : "Звіти";
        SubtitleTextBlock.Text = english ? "Analytics and statistics" : "Аналітика та статистика";
        DayPeriodComboBoxItem.Content = english ? "By day" : "За день";
        AllTimePeriodComboBoxItem.Content = english ? "All time" : "За весь час";
        OrdersCountLabelTextBlock.Text = english ? "Orders count" : "Кількість замовлень";
        RevenueLabelTextBlock.Text = english ? "Total revenue" : "Загальна виручка";
        AverageCheckLabelTextBlock.Text = english ? "Average check" : "Середній чек";
        ProductsCountLabelTextBlock.Text = english ? "Products in menu" : "Товарів у меню";
        ActiveProductsLabelTextBlock.Text = english ? "active items" : "активні позиції";
        TopProductsTitleTextBlock.Text = english ? "Top products" : "Топ товарів";
        EmptyTopProductsTextBlock.Text = english
            ? "No sales for this period"
            : "За цей період продажів немає";
    }

    private string GetChartTitle()
    {
        if (AppSettings.IsEnglish)
        {
            return IsAllTimeSelected ? "Revenue for all time" : "Revenue by day";
        }

        return IsAllTimeSelected ? "Виручка за весь час" : "Виручка за день";
    }
}

public sealed class TopProductViewModel
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string QuantityText => AppSettings.IsEnglish ? $"{Quantity} pcs." : $"{Quantity} шт.";
    public BitmapImage ImageSource { get; set; } = new();
}
