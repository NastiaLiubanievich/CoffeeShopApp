using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // Категорії
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Кава" },
            new Category { Id = 2, Name = "Холодні напої" },
            new Category { Id = 3, Name = "Сніданки" },
            new Category { Id = 4, Name = "Сендвічі та перекуси" },
            new Category { Id = 5, Name = "Випічка" },
            new Category { Id = 6, Name = "Десерти" }
        );

        var seedDate = new DateTime(2025, 1, 1);

        // Товари
        modelBuilder.Entity<Product>().HasData(
            // Кава
            new Product { Id = 1, Name = "Еспресо", Description = "Класичний міцний еспресо", Price = 25m, CategoryId = 1, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 2, Name = "Допіо", Description = "Подвійний еспресо", Price = 35m, CategoryId = 1, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 3, Name = "Американо", Description = "Еспресо з гарячою водою", Price = 30m, CategoryId = 1, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 4, Name = "Капучино", Description = "Еспресо з молоком та пінкою", Price = 40m, CategoryId = 1, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 5, Name = "Лате", Description = "Ніжна кава з молоком", Price = 45m, CategoryId = 1, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 6, Name = "Флет Вайт", Description = "Подвійний еспресо з молоком", Price = 50m, CategoryId = 1, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 7, Name = "Раф кава", Description = "Кава з вершками та ваніллю", Price = 55m, CategoryId = 1, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 8, Name = "Мокачино", Description = "Кава з шоколадом", Price = 55m, CategoryId = 1, IsAvailable = true, CreatedAt = seedDate },

            // Холодні напої
            new Product { Id = 9, Name = "Колд брю", Description = "Кава холодного заварювання", Price = 55m, CategoryId = 2, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 10, Name = "Колд брю тонік", Description = "Колд брю з тоніком", Price = 65m, CategoryId = 2, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 11, Name = "Айс лате", Description = "Холодне лате з льодом", Price = 55m, CategoryId = 2, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 12, Name = "Лимонад", Description = "Домашній лимонад", Price = 45m, CategoryId = 2, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 13, Name = "Апельсиновий фреш", Description = "Свіжовичавлений сік", Price = 60m, CategoryId = 2, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 14, Name = "Чай", Description = "Чорний або зелений чай", Price = 30m, CategoryId = 2, IsAvailable = true, CreatedAt = seedDate },

            // Сніданки
            new Product { Id = 15, Name = "Англійський сніданок", Description = "Яєчня, бекон, тости", Price = 120m, CategoryId = 3, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 16, Name = "Омлет з сиром", Description = "Ніжний омлет з сиром", Price = 85m, CategoryId = 3, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 17, Name = "Сирники", Description = "Сирники зі сметаною", Price = 95m, CategoryId = 3, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 18, Name = "Вівсянка з фруктами", Description = "Корисний сніданок", Price = 75m, CategoryId = 3, IsAvailable = true, CreatedAt = seedDate },

            // Сендвічі
            new Product { Id = 19, Name = "Сендвіч з куркою", Description = "Курка, сир, овочі", Price = 85m, CategoryId = 4, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 20, Name = "Паніні з шинкою", Description = "Теплий сендвіч", Price = 90m, CategoryId = 4, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 21, Name = "Тост з авокадо", Description = "Тост з авокадо та яйцем", Price = 110m, CategoryId = 4, IsAvailable = true, CreatedAt = seedDate },

            // Випічка
            new Product { Id = 22, Name = "Круасан", Description = "Французький круасан", Price = 35m, CategoryId = 5, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 23, Name = "Круасан з шоколадом", Description = "Круасан з шоколадною начинкою", Price = 45m, CategoryId = 5, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 24, Name = "Булочка з корицею", Description = "Ароматна булочка", Price = 42m, CategoryId = 5, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 25, Name = "Маффін шоколадний", Description = "Шоколадний маффін", Price = 38m, CategoryId = 5, IsAvailable = true, CreatedAt = seedDate },

            // Десерти
            new Product { Id = 26, Name = "Чізкейк", Description = "Класичний чізкейк", Price = 75m, CategoryId = 6, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 27, Name = "Тірамісу", Description = "Італійський десерт", Price = 80m, CategoryId = 6, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 28, Name = "Брауні", Description = "Шоколадний десерт", Price = 60m, CategoryId = 6, IsAvailable = true, CreatedAt = seedDate },
            new Product { Id = 29, Name = "Медівник", Description = "Домашній медовий торт", Price = 70m, CategoryId = 6, IsAvailable = true, CreatedAt = seedDate }
        );
    }
}

