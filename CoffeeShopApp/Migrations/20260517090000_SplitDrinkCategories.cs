using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopApp.Migrations;

[DbContext(typeof(CoffeeShopDbContext))]
[Migration("20260517090000_SplitDrinkCategories")]
public partial class SplitDrinkCategories : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE Categories
            SET Name = N'Гарячі напої'
            WHERE Id = 2;

            IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 7)
            BEGIN
                SET IDENTITY_INSERT Categories ON;

                INSERT INTO Categories (Id, Name)
                VALUES (7, N'Холодні напої');

                SET IDENTITY_INSERT Categories OFF;
            END;

            UPDATE Products
            SET CategoryId = 2
            WHERE Id IN (14, 42, 43);

            UPDATE Products
            SET CategoryId = 7
            WHERE Id IN (9, 10, 11, 12, 13, 32);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE Products
            SET CategoryId = 2
            WHERE Id IN (9, 10, 11, 12, 13, 14, 32, 42, 43);

            DELETE FROM Categories
            WHERE Id = 7;

            UPDATE Categories
            SET Name = N'Холодні напої'
            WHERE Id = 2;
            """);
    }
}
