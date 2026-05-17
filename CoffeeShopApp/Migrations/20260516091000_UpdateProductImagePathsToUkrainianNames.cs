using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopApp.Migrations;

[DbContext(typeof(CoffeeShopDbContext))]
[Migration("20260516091000_UpdateProductImagePathsToUkrainianNames")]
public partial class UpdateProductImagePathsToUkrainianNames : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE Products SET Name = N'Еспресо', ImagePath = N'Assets/Products/Еспресо.png' WHERE Id = 1;
            UPDATE Products SET Name = N'Допіо', ImagePath = N'Assets/Products/Допіо.png' WHERE Id = 2;
            UPDATE Products SET Name = N'Американо', ImagePath = N'Assets/Products/Американо.png' WHERE Id = 3;
            UPDATE Products SET Name = N'Капучино', ImagePath = N'Assets/Products/Капучино.png' WHERE Id = 4;
            UPDATE Products SET Name = N'Лате', ImagePath = N'Assets/Products/Лате.png' WHERE Id = 5;
            UPDATE Products SET Name = N'Флет вайт', ImagePath = N'Assets/Products/Флет вайт.png' WHERE Id = 6;
            UPDATE Products SET Name = N'Раф', ImagePath = N'Assets/Products/Раф.png' WHERE Id = 7;
            UPDATE Products SET Name = N'Мокачино', ImagePath = N'Assets/Products/Мокачино.png' WHERE Id = 8;
            UPDATE Products SET Name = N'Колд брю', ImagePath = N'Assets/Products/Колд брю.png' WHERE Id = 9;
            UPDATE Products SET Name = N'Еспресо тонік', Description = N'Еспресо з тоніком', ImagePath = N'Assets/Products/Еспресо тонік.png' WHERE Id = 10;
            UPDATE Products SET Name = N'Айс лате', ImagePath = N'Assets/Products/Айс лате.png' WHERE Id = 11;
            UPDATE Products SET Name = N'Лимонад', ImagePath = N'Assets/Products/Лимонад.png' WHERE Id = 12;
            UPDATE Products SET Name = N'Апельсиновий фреш', Description = N'Свіжовижатий сік', ImagePath = N'Assets/Products/Апельсиновий фреш.png' WHERE Id = 13;
            UPDATE Products SET Name = N'Чорний чай', Description = N'Чорний чай', ImagePath = N'Assets/Products/Чорний чай.png' WHERE Id = 14;
            UPDATE Products SET Name = N'Сендвіч з куркою', ImagePath = N'Assets/Products/Сендвіч з куркою.png' WHERE Id = 19;
            UPDATE Products SET Name = N'Паніні з шинкою', ImagePath = N'Assets/Products/Паніні з шинкою.png' WHERE Id = 20;
            UPDATE Products SET Name = N'Тост з авокадо', ImagePath = N'Assets/Products/Тост з авокадо.png' WHERE Id = 21;
            UPDATE Products SET Name = N'Круасан', ImagePath = N'Assets/Products/Круасан.png' WHERE Id = 22;
            UPDATE Products SET Name = N'Круасан з шоколадом', ImagePath = N'Assets/Products/Круасан з шоколадом.png' WHERE Id = 23;
            UPDATE Products SET Name = N'Сінабон', Description = N'Ароматна булочка', ImagePath = N'Assets/Products/Сінабон.png' WHERE Id = 24;
            UPDATE Products SET Name = N'Маффін шоколадний', ImagePath = N'Assets/Products/Маффін шоколадний.png' WHERE Id = 25;
            UPDATE Products SET Name = N'Чізкейк', ImagePath = N'Assets/Products/Чізкейк.png' WHERE Id = 26;
            UPDATE Products SET Name = N'Тірамісу', ImagePath = N'Assets/Products/Тірамісу.png' WHERE Id = 27;
            UPDATE Products SET Name = N'Брауні', ImagePath = N'Assets/Products/Брауні.png' WHERE Id = 28;
            UPDATE Products SET Name = N'Медівник', ImagePath = N'Assets/Products/Медівник.png' WHERE Id = 29;
            UPDATE Products SET Name = N'Макіато', ImagePath = N'Assets/Products/Макіато.png' WHERE Id = 30;
            UPDATE Products SET Name = N'Карамельний лате', ImagePath = N'Assets/Products/Карамельний лате.png' WHERE Id = 31;
            UPDATE Products SET Name = N'Айс матча лате', Description = N'Холодна матча з молоком', ImagePath = N'Assets/Products/Айс матча лате.png' WHERE Id = 32;
            UPDATE Products SET Name = N'Бейгл з лососем', ImagePath = N'Assets/Products/Бейгл з лососем.png' WHERE Id = 36;
            UPDATE Products SET Name = N'Овочевий рол', ImagePath = N'Assets/Products/Овочевий рол.png' WHERE Id = 37;
            UPDATE Products SET Name = N'Мигдалевий круасан', ImagePath = N'Assets/Products/Мигдалевий круасан.png' WHERE Id = 39;
            UPDATE Products SET Name = N'Еклер', ImagePath = N'Assets/Products/Еклер.png' WHERE Id = 40;
            UPDATE Products SET Name = N'Макаронс', ImagePath = N'Assets/Products/Макаронс.png' WHERE Id = 41;
            UPDATE Products SET Name = N'Какао', ImagePath = N'Assets/Products/Какао.png' WHERE Id = 42;
            UPDATE Products SET Name = N'Зелений чай', ImagePath = N'Assets/Products/Зелений чай.png' WHERE Id = 43;
            UPDATE Products SET Name = N'Брускета', ImagePath = N'Assets/Products/Брускета.png' WHERE Id = 44;
            UPDATE Products SET Name = N'Банановий хліб', ImagePath = N'Assets/Products/Банановий хліб.png' WHERE Id = 46;
            UPDATE Products SET Name = N'Фільтр кава', ImagePath = N'Assets/Products/Фільтр кава.png' WHERE Id = 48;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
