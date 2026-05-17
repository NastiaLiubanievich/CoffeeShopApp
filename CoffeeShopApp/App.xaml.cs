using System.Windows;

namespace CoffeeShopApp;

public partial class App : Application
{
    public static string CurrentUserName { get; set; } = "Адміністратор";
    public static string CurrentUserRole { get; set; } = "Адміністратор";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var loginWindow = new LoginWindow();
        loginWindow.Show();
    }
}
