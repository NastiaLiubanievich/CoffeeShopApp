using System.Windows;

namespace CoffeeShopApp;

public partial class PaymentMethodWindow : Window
{
    public string PaymentMethod { get; private set; } = string.Empty;

    public PaymentMethodWindow()
    {
        InitializeComponent();
    }

    private void CardButton_Click(object sender, RoutedEventArgs e)
    {
        PaymentMethod = "Картка";
        DialogResult = true;
    }

    private void CashButton_Click(object sender, RoutedEventArgs e)
    {
        PaymentMethod = "Готівка";
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
