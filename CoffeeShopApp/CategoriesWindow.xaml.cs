using System.Windows;
using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class CategoriesWindow : Window
{
    private readonly CoffeeShopDbContext _context = new();

    public CategoriesWindow()
    {
        InitializeComponent();
        CategoriesDataGrid.ItemsSource = _context.Categories
            .AsNoTracking()
            .Include(category => category.Products)
            .OrderBy(category => category.Id)
            .Select(category => new
            {
                category.Id,
                category.Name,
                ProductsCount = category.Products.Count
            })
            .ToList();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
