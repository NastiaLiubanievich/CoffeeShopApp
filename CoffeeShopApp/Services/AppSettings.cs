namespace CoffeeShopApp.Services;

public static class AppSettings
{
    public static event EventHandler? Changed;

    public static bool ShowOrderConfirmation { get; set; } = true;
    public static bool UseLightTheme { get; private set; } = true;
    public static bool IsEnglish => false;
    public static string ThemeName => UseLightTheme ? "Світла" : "Темна";

    public static DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }

    public static string FormatMoney(decimal amount)
    {
        return $"{amount:0} грн";
    }

    public static void SetTheme(string themeName)
    {
        UseLightTheme = themeName != "Темна";
        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static void NotifyChanged()
    {
        Changed?.Invoke(null, EventArgs.Empty);
    }
}
