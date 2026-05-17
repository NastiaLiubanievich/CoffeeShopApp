using System.Windows;
using System.Windows.Input;
using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        UsernameTextBox.Focus();
    }

    private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TryLogin();
        }
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        TryLogin();
    }

    private void TryLogin()
    {
        var username = UsernameTextBox.Text.Trim();
        var password = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Введіть логін і пароль.");
            return;
        }

        try
        {
            using var context = new CoffeeShopDbContext();
            var user = context.Users
                .AsNoTracking()
                .FirstOrDefault(account =>
                    account.IsActive &&
                    account.Username == username &&
                    account.Password == password);

            if (user is null)
            {
                ShowError("Неправильний логін або пароль.");
                return;
            }

            App.CurrentUserName = user.FullName;
            App.CurrentUserRole = user.Role;

            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        catch (Exception)
        {
            ShowError("Не вдалося підключитися до бази. Виконайте Update-Database.");
        }
    }

    private void ShowError(string message)
    {
        ErrorTextBlock.Text = message;
        ErrorTextBlock.Visibility = Visibility.Visible;
    }
}
