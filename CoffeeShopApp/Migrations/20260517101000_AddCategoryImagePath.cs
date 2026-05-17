using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopApp.Migrations;

[DbContext(typeof(CoffeeShopDbContext))]
[Migration("20260517101000_AddCategoryImagePath")]
public partial class AddCategoryImagePath : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ImagePath",
            table: "Categories",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.Sql("""
            UPDATE Categories SET ImagePath = N'Assets/Products/Капучино.png' WHERE Id = 1;
            UPDATE Categories SET ImagePath = N'Assets/Products/Какао.png' WHERE Id = 2;
            UPDATE Categories SET ImagePath = N'Assets/Products/Сендвіч з куркою.png' WHERE Id = 4;
            UPDATE Categories SET ImagePath = N'Assets/Products/Круасан.png' WHERE Id = 5;
            UPDATE Categories SET ImagePath = N'Assets/Products/Чізкейк.png' WHERE Id = 6;
            UPDATE Categories SET ImagePath = N'Assets/Products/Колд брю.png' WHERE Id = 7;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ImagePath",
            table: "Categories");
    }
}
