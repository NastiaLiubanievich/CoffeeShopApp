namespace CoffeeShopApp.Services;

public static class AppSettings
{
    public static bool ShowOrderConfirmation { get; set; } = true;
    public static bool UseLightTheme { get; set; } = true;
    public static string InterfaceLanguage { get; set; } = "Українська";
    public static string TimeZone { get; set; } = "(UTC+02:00) Київ";
    public static string Currency { get; set; } = "₴ Гривня";
    public static string ThemeName { get; set; } = "Світла";
}
