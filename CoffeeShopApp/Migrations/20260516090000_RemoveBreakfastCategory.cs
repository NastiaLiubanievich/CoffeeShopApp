using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopApp.Migrations
{
    [DbContext(typeof(CoffeeShopDbContext))]
    [Migration("20260516090000_RemoveBreakfastCategory")]
    public partial class RemoveBreakfastCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM OrderItems
                WHERE ProductId IN (15, 16, 17, 18, 33, 34, 35, 38, 45, 47);

                DELETE FROM Products
                WHERE Id IN (15, 16, 17, 18, 33, 34, 35, 38, 45, 47);

                DELETE FROM Categories
                WHERE Id = 3;

                UPDATE Products
                SET Name = N'Еспресо тонік',
                    Description = N'Еспресо з тоніком',
                    ImagePath = N'Assets/Products/cold-brew-tonic.png'
                WHERE Id = 10;

                UPDATE Products
                SET Name = N'Чорний чай',
                    Description = N'Чорний чай'
                WHERE Id = 14;

                UPDATE Products
                SET Name = N'Айс матча лате',
                    Description = N'Холодна матча з молоком'
                WHERE Id = 32;

                UPDATE Products
                SET Name = N'Сінабон',
                    Description = N'Ароматна булочка'
                WHERE Id = 24;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 3)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name) VALUES (3, N'Сніданки');
                    SET IDENTITY_INSERT Categories OFF;
                END
                """);
        }
    }
}
