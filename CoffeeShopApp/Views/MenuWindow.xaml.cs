using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class MenuWindow : Window
{
    private readonly CoffeeShopDbContext _context = new();
    private readonly List<MenuProductViewModel> _products = new();
    private int? _editingProductId;

    public MenuWindow()
    {
        InitializeComponent();
        LoadCategories();
        LoadProducts();
        ClearForm();
    }

    private void LoadCategories()
    {
        CategoryComboBox.ItemsSource = _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .ToList();
    }

    private void LoadProducts()
    {
        _products.Clear();
        _products.AddRange(_context.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .OrderBy(product => product.Id)
            .Select(product => new MenuProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Price = product.Price,
                ImagePath = product.ImagePath,
                IsAvailable = product.IsAvailable,
                ImageSource = ProductImageProvider.GetImageSource(product)
            })
            .ToList());

        ApplySearch();
    }

    private void ApplySearch()
    {
        var searchText = SearchTextBox.Text.Trim();
        var products = _products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            products = products.Where(product =>
                product.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                product.CategoryName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));
        }

        ProductsDataGrid.ItemsSource = products.ToList();
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplySearch();
    }

    private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProductsDataGrid.SelectedItem is MenuProductViewModel product)
        {
            FillForm(product);
        }
    }

    private void AddNewButton_Click(object sender, RoutedEventArgs e)
    {
        ClearForm();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: int productId })
        {
            var product = _products.FirstOrDefault(item => item.Id == productId);
            if (product is not null)
            {
                FillForm(product);
            }
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: int productId })
        {
            return;
        }

        var product = _context.Products.FirstOrDefault(item => item.Id == productId);
        if (product is null)
        {
            return;
        }

        var result = MessageBox.Show(
            $"Видалити товар \"{product.Name}\"?",
            "Меню",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        _context.Products.Remove(product);
        _context.SaveChanges();
        LoadProducts();
        ClearForm();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadForm(out var name, out var description, out var price, out var categoryId, out var imagePath, out var isAvailable))
        {
            return;
        }

        Product product;
        if (_editingProductId is null)
        {
            product = new Product
            {
                CreatedAt = DateTime.Now
            };
            _context.Products.Add(product);
        }
        else
        {
            product = _context.Products.First(item => item.Id == _editingProductId.Value);
        }

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.CategoryId = categoryId;
        product.ImagePath = imagePath;
        product.IsAvailable = isAvailable;

        _context.SaveChanges();
        LoadProducts();
        ClearForm();
    }

    private bool TryReadForm(
        out string name,
        out string description,
        out decimal price,
        out int categoryId,
        out string imagePath,
        out bool isAvailable)
    {
        name = NameTextBox.Text.Trim();
        description = DescriptionTextBox.Text.Trim();
        imagePath = ImagePathTextBox.Text.Trim();
        isAvailable = AvailableCheckBox.IsChecked == true;
        categoryId = CategoryComboBox.SelectedValue is int selectedCategoryId ? selectedCategoryId : 0;

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Введіть назву товару.", "Меню", MessageBoxButton.OK, MessageBoxImage.Information);
            price = 0;
            return false;
        }

        if (categoryId == 0)
        {
            MessageBox.Show("Оберіть категорію.", "Меню", MessageBoxButton.OK, MessageBoxImage.Information);
            price = 0;
            return false;
        }

        if (!decimal.TryParse(PriceTextBox.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out price))
        {
            MessageBox.Show("Введіть коректну ціну.", "Меню", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            description = "Без опису";
        }

        if (string.IsNullOrWhiteSpace(imagePath))
        {
            imagePath = $"Assets/Products/{name}.png";
        }

        return true;
    }

    private void FillForm(MenuProductViewModel product)
    {
        _editingProductId = product.Id;
        FormTitleTextBlock.Text = "Редагувати товар";
        NameTextBox.Text = product.Name;
        DescriptionTextBox.Text = product.Description;
        PriceTextBox.Text = product.Price.ToString("0.##");
        ImagePathTextBox.Text = product.ImagePath;
        AvailableCheckBox.IsChecked = product.IsAvailable;
        CategoryComboBox.SelectedValue = product.CategoryId;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        _editingProductId = null;
        ProductsDataGrid.SelectedItem = null;
        FormTitleTextBlock.Text = "Додати товар";
        NameTextBox.Clear();
        DescriptionTextBox.Clear();
        PriceTextBox.Clear();
        ImagePathTextBox.Clear();
        AvailableCheckBox.IsChecked = true;
        CategoryComboBox.SelectedIndex = CategoryComboBox.Items.Count > 0 ? 0 : -1;
    }
}

public sealed class MenuProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public BitmapImage ImageSource { get; set; } = new();
}
