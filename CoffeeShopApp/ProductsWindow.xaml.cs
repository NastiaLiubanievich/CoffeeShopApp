using System.Windows;
using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class ProductsWindow : Window
{
    private readonly CoffeeShopDbContext _context = new();

    public ProductsWindow()
    {
        InitializeComponent();
        LoadProducts();
    }

    private void LoadProducts()
    {
        ProductsDataGrid.ItemsSource = _context.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .OrderBy(product => product.Id)
            .Select(product => new
            {
                product.Id,
                product.Name,
                product.ImagePath,
                Category = product.Category.Name,
                product.Description,
                product.Price,
                product.IsAvailable
            })
            .ToList();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadProducts();

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
