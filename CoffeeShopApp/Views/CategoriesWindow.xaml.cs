using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class CategoriesWindow : Window
{
    private readonly CoffeeShopDbContext _context = new();

    public CategoriesWindow()
    {
        InitializeComponent();
        LoadCategories();
    }

    private void LoadCategories()
    {
        CategoriesItemsControl.ItemsSource = _context.Categories
            .AsNoTracking()
            .Include(category => category.Products)
            .OrderBy(category => category.Id)
            .Select(category => new CategoryCardViewModel
            {
                Id = category.Id,
                Name = GetShortCategoryName(category.Name),
                ProductsCount = category.Products.Count,
                ImageSource = GetCategoryImage(category),
                ImagePath = category.ImagePath
            })
            .ToList();
    }

    private static string GetShortCategoryName(string name)
    {
        return name switch
        {
            "Сендвічі та перекуси" => "Перекуси",
            _ => name
        };
    }

    private static BitmapImage GetCategoryImage(Category category)
    {
        if (!string.IsNullOrWhiteSpace(category.ImagePath))
        {
            return ProductImageProvider.GetImageSource(new Product
            {
                Id = 0,
                Name = category.Name,
                ImagePath = category.ImagePath
            });
        }

        var imagePath = category.Id switch
        {
            1 => "Assets/Products/Капучино.png",
            2 => "Assets/Products/Какао.png",
            4 => "Assets/Products/Сендвіч з куркою.png",
            5 => "Assets/Products/Круасан.png",
            6 => "Assets/Products/Чізкейк.png",
            7 => "Assets/Products/Колд брю.png",
            _ => "Assets/Products/Еспресо.png"
        };

        return ProductImageProvider.GetImageSource(new Product
        {
            Id = 0,
            Name = category.Name,
            ImagePath = imagePath
        });
    }

    private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new CategoryEditWindow
        {
            Owner = this
        };

        if (window.ShowDialog() != true)
        {
            return;
        }

        _context.Categories.Add(new Category
        {
            Name = window.CategoryName,
            ImagePath = string.IsNullOrWhiteSpace(window.ImagePath)
                ? "Assets/Products/Еспресо.png"
                : window.ImagePath
        });
        _context.SaveChanges();
        LoadCategories();
    }

    private void EditCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: int categoryId })
        {
            return;
        }

        var category = _context.Categories.FirstOrDefault(item => item.Id == categoryId);
        if (category is null)
        {
            return;
        }

        var window = new CategoryEditWindow(category.Name, category.ImagePath)
        {
            Owner = this
        };

        if (window.ShowDialog() != true)
        {
            return;
        }

        category.Name = window.CategoryName;
        category.ImagePath = string.IsNullOrWhiteSpace(window.ImagePath)
            ? "Assets/Products/Еспресо.png"
            : window.ImagePath;
        _context.SaveChanges();
        LoadCategories();
    }

    private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: int categoryId })
        {
            return;
        }

        var category = _context.Categories
            .Include(item => item.Products)
            .FirstOrDefault(item => item.Id == categoryId);

        if (category is null)
        {
            return;
        }

        if (category.Products.Count > 0)
        {
            MessageBox.Show("Не можна видалити категорію, у якій є товари.", "Категорії", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();
        LoadCategories();
    }
}

public sealed class CategoryCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProductsCount { get; set; }
    public string ProductsCountText => $"{ProductsCount} товарів";
    public string ImagePath { get; set; } = string.Empty;
    public BitmapImage ImageSource { get; set; } = new();
}
