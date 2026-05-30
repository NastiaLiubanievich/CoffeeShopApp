using CoffeeShopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApp.Data;

public class CoffeeShopDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<UserAccount> Users => Set<UserAccount>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\MSSQLLocalDB;Database=CoffeeShopDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Кава", ImagePath = "Assets/Products/Капучино.png" },
            new Category { Id = 2, Name = "Гарячі напої", ImagePath = "Assets/Products/Какао.png" },
            new Category { Id = 4, Name = "Сендвічі та перекуси", ImagePath = "Assets/Products/Сендвіч з куркою.png" },
            new Category { Id = 5, Name = "Випічка", ImagePath = "Assets/Products/Круасан.png" },
            new Category { Id = 6, Name = "Десерти", ImagePath = "Assets/Products/Чізкейк.png" },
            new Category { Id = 7, Name = "Холодні напої", ImagePath = "Assets/Products/Колд брю.png" }
        );

        var seedDate = new DateTime(2025, 1, 1);

        Product Product(int id, string name, string description, decimal price, int categoryId)
        {
            return new Product
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                ImagePath = $"Assets/Products/{name}.png",
                IsAvailable = true,
                CreatedAt = seedDate
            };
        }

        modelBuilder.Entity<Product>().HasData(
            Product(1, "Еспресо", "Класичний міцний еспресо", 25m, 1),
            Product(2, "Допіо", "Подвійний еспресо", 35m, 1),
            Product(3, "Американо", "Еспресо з гарячою водою", 30m, 1),
            Product(4, "Капучино", "Еспресо з молоком та пінкою", 40m, 1),
            Product(5, "Лате", "Ніжна кава з молоком", 45m, 1),
            Product(6, "Флет вайт", "Подвійний еспресо з молоком", 50m, 1),
            Product(7, "Раф", "Кава з вершками та ваніллю", 55m, 1),
            Product(8, "Мокачино", "Кава з шоколадом", 55m, 1),
            Product(30, "Макіато", "Еспресо з молочною пінкою", 42m, 1),
            Product(31, "Карамельний лате", "Лате з карамельним сиропом", 62m, 1),
            Product(48, "Фільтр кава", "Чорна кава ручного заварювання", 48m, 1),

            Product(14, "Чорний чай", "Чорний чай", 30m, 2),
            Product(42, "Какао", "Гаряче какао з молоком", 48m, 2),
            Product(43, "Зелений чай", "Китайський зелений чай", 35m, 2),

            Product(9, "Колд брю", "Кава холодного заварювання", 55m, 7),
            Product(10, "Еспресо тонік", "Еспресо з тоніком", 65m, 7),
            Product(11, "Айс лате", "Холодне лате з льодом", 55m, 7),
            Product(12, "Лимонад", "Домашній лимонад", 45m, 7),
            Product(13, "Апельсиновий фреш", "Свіжовижатий сік", 60m, 7),
            Product(32, "Айс матча лате", "Холодна матча з молоком", 68m, 7),

            Product(19, "Сендвіч з куркою", "Курка, сир, овочі", 85m, 4),
            Product(20, "Паніні з шинкою", "Теплий сендвіч", 90m, 4),
            Product(21, "Тост з авокадо", "Тост з авокадо та яйцем", 110m, 4),
            Product(36, "Бейгл з лососем", "Бейгл з крем-сиром та лососем", 135m, 4),
            Product(37, "Овочевий рол", "Рол з овочами та соусом", 82m, 4),
            Product(44, "Брускета", "Тости з томатами та сиром", 78m, 4),

            Product(22, "Круасан", "Французький круасан", 35m, 5),
            Product(23, "Круасан з шоколадом", "Круасан з шоколадною начинкою", 45m, 5),
            Product(24, "Сінабон", "Ароматна булочка", 42m, 5),
            Product(25, "Маффін шоколадний", "Шоколадний маффін", 38m, 5),
            Product(39, "Мигдалевий круасан", "Круасан з мигдалевим кремом", 58m, 5),
            Product(46, "Банановий хліб", "Солодка випічка з бананом", 52m, 5),

            Product(26, "Чізкейк", "Класичний чізкейк", 75m, 6),
            Product(27, "Тірамісу", "Італійський десерт", 80m, 6),
            Product(28, "Брауні", "Шоколадний десерт", 60m, 6),
            Product(29, "Медівник", "Домашній медовий торт", 70m, 6),
            Product(40, "Еклер", "Еклер із заварним кремом", 62m, 6),
            Product(41, "Макаронс", "Набір мигдалевого печива", 90m, 6)
        );

        modelBuilder.Entity<UserAccount>().HasData(
            new UserAccount
            {
                Id = 1,
                FullName = "Адміністратор",
                Username = "admin",
                Password = "admin123",
                Role = "Адміністратор",
                IsActive = true
            },
            new UserAccount
            {
                Id = 2,
                FullName = "Касир",
                Username = "cashier",
                Password = "cashier123",
                Role = "Касир",
                IsActive = true
            },
            new UserAccount
            {
                Id = 3,
                FullName = "Менеджер",
                Username = "manager",
                Password = "manager123",
                Role = "Менеджер",
                IsActive = true
            }
        );
    }
}
