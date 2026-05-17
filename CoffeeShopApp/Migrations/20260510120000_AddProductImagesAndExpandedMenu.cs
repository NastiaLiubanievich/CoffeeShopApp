using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopApp.Migrations
{
    [DbContext(typeof(CoffeeShopDbContext))]
    [Migration("20260510120000_AddProductImagesAndExpandedMenu")]
    public partial class AddProductImagesAndExpandedMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE Products SET ImagePath = N'Assets/Products/espresso.png' WHERE Id = 1;
                UPDATE Products SET ImagePath = N'Assets/Products/dopio.png' WHERE Id = 2;
                UPDATE Products SET ImagePath = N'Assets/Products/americano.png' WHERE Id = 3;
                UPDATE Products SET ImagePath = N'Assets/Products/cappuccino.png' WHERE Id = 4;
                UPDATE Products SET ImagePath = N'Assets/Products/latte.png' WHERE Id = 5;
                UPDATE Products SET ImagePath = N'Assets/Products/flat-white.png' WHERE Id = 6;
                UPDATE Products SET ImagePath = N'Assets/Products/raf.png' WHERE Id = 7;
                UPDATE Products SET ImagePath = N'Assets/Products/mochaccino.png' WHERE Id = 8;
                UPDATE Products SET ImagePath = N'Assets/Products/cold-brew.png' WHERE Id = 9;
                UPDATE Products SET ImagePath = N'Assets/Products/cold-brew-tonic.png' WHERE Id = 10;
                UPDATE Products SET ImagePath = N'Assets/Products/ice-latte.png' WHERE Id = 11;
                UPDATE Products SET ImagePath = N'Assets/Products/lemonade.png' WHERE Id = 12;
                UPDATE Products SET ImagePath = N'Assets/Products/orange-fresh.png' WHERE Id = 13;
                UPDATE Products SET ImagePath = N'Assets/Products/tea.png' WHERE Id = 14;
                UPDATE Products SET ImagePath = N'Assets/Products/english-breakfast.png' WHERE Id = 15;
                UPDATE Products SET ImagePath = N'Assets/Products/cheese-omelet.png' WHERE Id = 16;
                UPDATE Products SET ImagePath = N'Assets/Products/syrnyky.png' WHERE Id = 17;
                UPDATE Products SET ImagePath = N'Assets/Products/oatmeal.png' WHERE Id = 18;
                UPDATE Products SET ImagePath = N'Assets/Products/chicken-sandwich.png' WHERE Id = 19;
                UPDATE Products SET ImagePath = N'Assets/Products/ham-panini.png' WHERE Id = 20;
                UPDATE Products SET ImagePath = N'Assets/Products/avocado-toast.png' WHERE Id = 21;
                UPDATE Products SET ImagePath = N'Assets/Products/croissant.png' WHERE Id = 22;
                UPDATE Products SET ImagePath = N'Assets/Products/chocolate-croissant.png' WHERE Id = 23;
                UPDATE Products SET ImagePath = N'Assets/Products/cinnamon-roll.png' WHERE Id = 24;
                UPDATE Products SET ImagePath = N'Assets/Products/chocolate-muffin.png' WHERE Id = 25;
                UPDATE Products SET ImagePath = N'Assets/Products/cheesecake.png' WHERE Id = 26;
                UPDATE Products SET ImagePath = N'Assets/Products/tiramisu.png' WHERE Id = 27;
                UPDATE Products SET ImagePath = N'Assets/Products/brownie.png' WHERE Id = 28;
                UPDATE Products SET ImagePath = N'Assets/Products/honey-cake.png' WHERE Id = 29;
                """);

            migrationBuilder.Sql("""
                SET IDENTITY_INSERT Products ON;

                IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = 30)
                BEGIN
                INSERT INTO Products (Id, Name, Description, ImagePath, Price, CategoryId, IsAvailable, CreatedAt)
                VALUES
                (30, N'Макіато', N'Еспресо з молочною пінкою', N'Assets/Products/macchiato.png', 42, 1, 1, '2025-01-01'),
                (31, N'Карамельний лате', N'Лате з карамельним сиропом', N'Assets/Products/caramel-latte.png', 62, 1, 1, '2025-01-01'),
                (32, N'Матча лате', N'Матча з молоком', N'Assets/Products/matcha-latte.png', 68, 2, 1, '2025-01-01'),
                (33, N'Ягідний смузі', N'Смузі з ягодами та бананом', N'Assets/Products/berry-smoothie.png', 72, 2, 1, '2025-01-01'),
                (34, N'Гранола', N'Гранола з йогуртом та ягодами', N'Assets/Products/granola.png', 88, 3, 1, '2025-01-01'),
                (35, N'Панкейки', N'Панкейки з сиропом', N'Assets/Products/pancakes.png', 98, 3, 1, '2025-01-01'),
                (36, N'Бейгл з лососем', N'Бейгл з крем-сиром та лососем', N'Assets/Products/salmon-bagel.png', 135, 4, 1, '2025-01-01'),
                (37, N'Овочевий рол', N'Рол з овочами та соусом', N'Assets/Products/veggie-wrap.png', 82, 4, 1, '2025-01-01'),
                (38, N'Багет', N'Хрусткий міні-багет', N'Assets/Products/baguette.png', 32, 5, 1, '2025-01-01'),
                (39, N'Мигдалевий круасан', N'Круасан з мигдалевим кремом', N'Assets/Products/almond-croissant.png', 58, 5, 1, '2025-01-01'),
                (40, N'Еклер', N'Еклер із заварним кремом', N'Assets/Products/eclair.png', 62, 6, 1, '2025-01-01'),
                (41, N'Макаронс', N'Набір мигдалевого печива', N'Assets/Products/macaron-set.png', 90, 6, 1, '2025-01-01'),
                (42, N'Какао', N'Гаряче какао з молоком', N'Assets/Products/cocoa.png', 48, 2, 1, '2025-01-01'),
                (43, N'Зелений чай', N'Китайський зелений чай', N'Assets/Products/green-tea.png', 35, 2, 1, '2025-01-01'),
                (44, N'Брускета', N'Тости з томатами та сиром', N'Assets/Products/bruschetta.png', 78, 4, 1, '2025-01-01'),
                (45, N'Кіш з куркою', N'Солоний пиріг з куркою', N'Assets/Products/quiche.png', 92, 3, 1, '2025-01-01'),
                (46, N'Банановий хліб', N'Солодка випічка з бананом', N'Assets/Products/banana-bread.png', 52, 5, 1, '2025-01-01'),
                (47, N'Шоколадний мус', N'Легкий мус з чорного шоколаду', N'Assets/Products/mousse.png', 78, 6, 1, '2025-01-01'),
                (48, N'Фільтр кава', N'Чорна кава ручного заварювання', N'Assets/Products/filter-coffee.png', 48, 1, 1, '2025-01-01');
                END

                SET IDENTITY_INSERT Products OFF;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Products WHERE Id BETWEEN 30 AND 48;");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Products");
        }
    }
}
