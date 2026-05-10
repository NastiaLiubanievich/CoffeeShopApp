using CoffeeShopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp.Data;

public class CoffeeShopDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\MSSQLLocalDB;Database=CoffeeShopDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Кава" },
            new Category { Id = 2, Name = "Холодні напої" },
            new Category { Id = 3, Name = "Сніданки" },
            new Category { Id = 4, Name = "Сендвічі та перекуси" },
            new Category { Id = 5, Name = "Випічка" },
            new Category { Id = 6, Name = "Десерти" }
        );

        var seedDate = new DateTime(2025, 1, 1);

        Product Product(int id, string name, string description, decimal price, int categoryId, string imageName)
        {
            return new Product
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                ImagePath = $"Assets/Products/{imageName}.png",
                IsAvailable = true,
                CreatedAt = seedDate
            };
        }

        modelBuilder.Entity<Product>().HasData(
            Product(1, "Еспресо", "Класичний міцний еспресо", 25m, 1, "espresso"),
            Product(2, "Допіо", "Подвійний еспресо", 35m, 1, "dopio"),
            Product(3, "Американо", "Еспресо з гарячою водою", 30m, 1, "americano"),
            Product(4, "Капучино", "Еспресо з молоком та пінкою", 40m, 1, "cappuccino"),
            Product(5, "Лате", "Ніжна кава з молоком", 45m, 1, "latte"),
            Product(6, "Флет Вайт", "Подвійний еспресо з молоком", 50m, 1, "flat-white"),
            Product(7, "Раф кава", "Кава з вершками та ваніллю", 55m, 1, "raf"),
            Product(8, "Мокачино", "Кава з шоколадом", 55m, 1, "mochaccino"),
            Product(30, "Макіато", "Еспресо з молочною пінкою", 42m, 1, "macchiato"),
            Product(31, "Карамельний лате", "Лате з карамельним сиропом", 62m, 1, "caramel-latte"),
            Product(48, "Фільтр кава", "Чорна кава ручного заварювання", 48m, 1, "filter-coffee"),

            Product(9, "Колд брю", "Кава холодного заварювання", 55m, 2, "cold-brew"),
            Product(10, "Колд брю тонік", "Колд брю з тоніком", 65m, 2, "cold-brew-tonic"),
            Product(11, "Айс лате", "Холодне лате з льодом", 55m, 2, "ice-latte"),
            Product(12, "Лимонад", "Домашній лимонад", 45m, 2, "lemonade"),
            Product(13, "Апельсиновий фреш", "Свіжовичавлений сік", 60m, 2, "orange-fresh"),
            Product(14, "Чай", "Чорний або зелений чай", 30m, 2, "tea"),
            Product(32, "Матча лате", "Матча з молоком", 68m, 2, "matcha-latte"),
            Product(33, "Ягідний смузі", "Смузі з ягодами та бананом", 72m, 2, "berry-smoothie"),
            Product(42, "Какао", "Гаряче какао з молоком", 48m, 2, "cocoa"),
            Product(43, "Зелений чай", "Китайський зелений чай", 35m, 2, "green-tea"),

            Product(15, "Англійський сніданок", "Яєчня, бекон, тости", 120m, 3, "english-breakfast"),
            Product(16, "Омлет з сиром", "Ніжний омлет з сиром", 85m, 3, "cheese-omelet"),
            Product(17, "Сирники", "Сирники зі сметаною", 95m, 3, "syrnyky"),
            Product(18, "Вівсянка з фруктами", "Корисний сніданок", 75m, 3, "oatmeal"),
            Product(34, "Гранола", "Гранола з йогуртом та ягодами", 88m, 3, "granola"),
            Product(35, "Панкейки", "Панкейки з сиропом", 98m, 3, "pancakes"),
            Product(45, "Кіш з куркою", "Солоний пиріг з куркою", 92m, 3, "quiche"),

            Product(19, "Сендвіч з куркою", "Курка, сир, овочі", 85m, 4, "chicken-sandwich"),
            Product(20, "Паніні з шинкою", "Теплий сендвіч", 90m, 4, "ham-panini"),
            Product(21, "Тост з авокадо", "Тост з авокадо та яйцем", 110m, 4, "avocado-toast"),
            Product(36, "Бейгл з лососем", "Бейгл з крем-сиром та лососем", 135m, 4, "salmon-bagel"),
            Product(37, "Овочевий рол", "Рол з овочами та соусом", 82m, 4, "veggie-wrap"),
            Product(44, "Брускета", "Тости з томатами та сиром", 78m, 4, "bruschetta"),

            Product(22, "Круасан", "Французький круасан", 35m, 5, "croissant"),
            Product(23, "Круасан з шоколадом", "Круасан з шоколадною начинкою", 45m, 5, "chocolate-croissant"),
            Product(24, "Булочка з корицею", "Ароматна булочка", 42m, 5, "cinnamon-roll"),
            Product(25, "Маффін шоколадний", "Шоколадний маффін", 38m, 5, "chocolate-muffin"),
            Product(38, "Багет", "Хрусткий міні-багет", 32m, 5, "baguette"),
            Product(39, "Мигдалевий круасан", "Круасан з мигдалевим кремом", 58m, 5, "almond-croissant"),
            Product(46, "Банановий хліб", "Солодка випічка з бананом", 52m, 5, "banana-bread"),

            Product(26, "Чізкейк", "Класичний чізкейк", 75m, 6, "cheesecake"),
            Product(27, "Тірамісу", "Італійський десерт", 80m, 6, "tiramisu"),
            Product(28, "Брауні", "Шоколадний десерт", 60m, 6, "brownie"),
            Product(29, "Медівник", "Домашній медовий торт", 70m, 6, "honey-cake"),
            Product(40, "Еклер", "Еклер із заварним кремом", 62m, 6, "eclair"),
            Product(41, "Макаронс", "Набір мигдалевого печива", 90m, 6, "macaron-set"),
            Product(47, "Шоколадний мус", "Легкий мус з чорного шоколаду", 78m, 6, "mousse")
        );
    }
}
