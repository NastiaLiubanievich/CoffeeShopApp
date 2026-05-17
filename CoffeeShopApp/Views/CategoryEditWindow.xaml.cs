using System.Windows;

namespace CoffeeShopApp;

public partial class CategoryEditWindow : Window
{
    public CategoryEditWindow(string? categoryName = null, string? imagePath = null)
    {
        InitializeComponent();

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            TitleTextBlock.Text = "Редагувати категорію";
            NameTextBox.Text = categoryName;
        }

        ImagePathTextBox.Text = imagePath ?? string.Empty;
        NameTextBox.Focus();
        NameTextBox.SelectAll();
    }

    public string CategoryName => NameTextBox.Text.Trim();
    public string ImagePath => ImagePathTextBox.Text.Trim();

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CategoryName))
        {
            MessageBox.Show("Введіть назву категорії.", "Категорія", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
