using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class MenuView : UserControl
{
    private readonly CoffeeShopDbContext _context = new();
    private readonly List<MenuProductViewModel> _products = [];
    private int? _editingProductId;

    public MenuView()
    {
        InitializeComponent();
        ApplyLanguage();
        LoadCategories();
        LoadProducts();
        ClearForm();
    }

    private void LoadCategories()
    {
        CategoryComboBox.ItemsSource = _context.Categories.AsNoTracking().OrderBy(category => category.Id).ToList();
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
        SearchPlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
        ApplySearch();
    }

    private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProductsDataGrid.SelectedItem is MenuProductViewModel product)
        {
            FillForm(product);
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: int productId } &&
            _products.FirstOrDefault(item => item.Id == productId) is { } product)
        {
            FillForm(product);
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

        if (MessageBox.Show(
                AppSettings.IsEnglish ? $"Delete product \"{product.Name}\"?" : $"Видалити товар \"{product.Name}\"?",
                AppSettings.IsEnglish ? "Menu" : "Меню",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
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
            product = new Product { CreatedAt = DateTime.Now };
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

    private bool TryReadForm(out string name, out string description, out decimal price, out int categoryId, out string imagePath, out bool isAvailable)
    {
        name = NameTextBox.Text.Trim();
        description = DescriptionTextBox.Text.Trim();
        imagePath = ImagePathTextBox.Text.Trim();
        isAvailable = AvailableCheckBox.IsChecked == true;
        categoryId = CategoryComboBox.SelectedValue is int selectedCategoryId ? selectedCategoryId : 0;

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(
                AppSettings.IsEnglish ? "Enter product name." : "Введіть назву товару.",
                AppSettings.IsEnglish ? "Menu" : "Меню",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            price = 0;
            return false;
        }

        if (categoryId == 0)
        {
            MessageBox.Show(
                AppSettings.IsEnglish ? "Choose category." : "Оберіть категорію.",
                AppSettings.IsEnglish ? "Menu" : "Меню",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            price = 0;
            return false;
        }

        if (!decimal.TryParse(PriceTextBox.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out price))
        {
            MessageBox.Show(
                AppSettings.IsEnglish ? "Enter a valid price." : "Введіть коректну ціну.",
                AppSettings.IsEnglish ? "Menu" : "Меню",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return false;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            description = AppSettings.IsEnglish ? "No description" : "Без опису";
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
        FormTitleTextBlock.Text = AppSettings.IsEnglish ? "Edit product" : "Редагувати товар";
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
        FormTitleTextBlock.Text = AppSettings.IsEnglish ? "Add product" : "Додати товар";
        NameTextBox.Clear();
        DescriptionTextBox.Clear();
        PriceTextBox.Clear();
        ImagePathTextBox.Clear();
        AvailableCheckBox.IsChecked = true;
        CategoryComboBox.SelectedIndex = CategoryComboBox.Items.Count > 0 ? 0 : -1;
    }

    private void ApplyLanguage()
    {
        var english = AppSettings.IsEnglish;

        TitleTextBlock.Text = english ? "Menu" : "Меню";
        SubtitleTextBlock.Text = english ? "Product management" : "Управління товарами";
        SearchPlaceholderTextBlock.Text = english ? "Search products..." : "Пошук товарів...";

        PhotoColumn.Header = english ? "Photo" : "Фото";
        NameColumn.Header = english ? "Name" : "Назва";
        CategoryColumn.Header = english ? "Category" : "Категорія";
        PriceColumn.Header = english ? "Price" : "Ціна";
        AvailableColumn.Header = english ? "Available" : "Доступний";
        ActionsColumn.Header = english ? "Actions" : "Дії";

        NameLabelTextBlock.Text = english ? "Product name" : "Назва товару";
        CategoryLabelTextBlock.Text = english ? "Category" : "Категорія";
        PriceLabelTextBlock.Text = english ? "Price (грн)" : "Ціна (грн)";
        DescriptionLabelTextBlock.Text = english ? "Description" : "Опис";
        ImagePathLabelTextBlock.Text = english ? "Photo path" : "Шлях до фото";
        AvailableCheckBox.Content = english ? "Available" : "Доступний";
        CancelButton.Content = english ? "Cancel" : "Скасувати";
        SaveButton.Content = english ? "Save" : "Зберегти";
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
    public string PriceText => AppSettings.FormatMoney(Price);
    public string ImagePath { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public BitmapImage ImageSource { get; set; } = new();
}
