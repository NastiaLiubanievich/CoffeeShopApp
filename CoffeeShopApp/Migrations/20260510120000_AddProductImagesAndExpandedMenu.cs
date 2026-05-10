using System;
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

            SetImage(migrationBuilder, 1, "espresso");
            SetImage(migrationBuilder, 2, "dopio");
            SetImage(migrationBuilder, 3, "americano");
            SetImage(migrationBuilder, 4, "cappuccino");
            SetImage(migrationBuilder, 5, "latte");
            SetImage(migrationBuilder, 6, "flat-white");
            SetImage(migrationBuilder, 7, "raf");
            SetImage(migrationBuilder, 8, "mochaccino");
            SetImage(migrationBuilder, 9, "cold-brew");
            SetImage(migrationBuilder, 10, "cold-brew-tonic");
            SetImage(migrationBuilder, 11, "ice-latte");
            SetImage(migrationBuilder, 12, "lemonade");
            SetImage(migrationBuilder, 13, "orange-fresh");
            SetImage(migrationBuilder, 14, "tea");
            SetImage(migrationBuilder, 15, "english-breakfast");
            SetImage(migrationBuilder, 16, "cheese-omelet");
            SetImage(migrationBuilder, 17, "syrnyky");
            SetImage(migrationBuilder, 18, "oatmeal");
            SetImage(migrationBuilder, 19, "chicken-sandwich");
            SetImage(migrationBuilder, 20, "ham-panini");
            SetImage(migrationBuilder, 21, "avocado-toast");
            SetImage(migrationBuilder, 22, "croissant");
            SetImage(migrationBuilder, 23, "chocolate-croissant");
            SetImage(migrationBuilder, 24, "cinnamon-roll");
            SetImage(migrationBuilder, 25, "chocolate-muffin");
            SetImage(migrationBuilder, 26, "cheesecake");
            SetImage(migrationBuilder, 27, "tiramisu");
            SetImage(migrationBuilder, 28, "brownie");
            SetImage(migrationBuilder, 29, "honey-cake");

            var seedDate = new DateTime(2025, 1, 1);

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImagePath", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 30, 1, seedDate, "Еспресо з молочною пінкою", "Assets/Products/macchiato.png", true, "Макіато", 42m },
                    { 31, 1, seedDate, "Лате з карамельним сиропом", "Assets/Products/caramel-latte.png", true, "Карамельний лате", 62m },
                    { 32, 2, seedDate, "Матча з молоком", "Assets/Products/matcha-latte.png", true, "Матча лате", 68m },
                    { 33, 2, seedDate, "Смузі з ягодами та бананом", "Assets/Products/berry-smoothie.png", true, "Ягідний смузі", 72m },
                    { 34, 3, seedDate, "Гранола з йогуртом та ягодами", "Assets/Products/granola.png", true, "Гранола", 88m },
                    { 35, 3, seedDate, "Панкейки з сиропом", "Assets/Products/pancakes.png", true, "Панкейки", 98m },
                    { 36, 4, seedDate, "Бейгл з крем-сиром та лососем", "Assets/Products/salmon-bagel.png", true, "Бейгл з лососем", 135m },
                    { 37, 4, seedDate, "Рол з овочами та соусом", "Assets/Products/veggie-wrap.png", true, "Овочевий рол", 82m },
                    { 38, 5, seedDate, "Хрусткий міні-багет", "Assets/Products/baguette.png", true, "Багет", 32m },
                    { 39, 5, seedDate, "Круасан з мигдалевим кремом", "Assets/Products/almond-croissant.png", true, "Мигдалевий круасан", 58m },
                    { 40, 6, seedDate, "Еклер із заварним кремом", "Assets/Products/eclair.png", true, "Еклер", 62m },
                    { 41, 6, seedDate, "Набір мигдалевого печива", "Assets/Products/macaron-set.png", true, "Макаронс", 90m },
                    { 42, 2, seedDate, "Гаряче какао з молоком", "Assets/Products/cocoa.png", true, "Какао", 48m },
                    { 43, 2, seedDate, "Китайський зелений чай", "Assets/Products/green-tea.png", true, "Зелений чай", 35m },
                    { 44, 4, seedDate, "Тости з томатами та сиром", "Assets/Products/bruschetta.png", true, "Брускета", 78m },
                    { 45, 3, seedDate, "Солоний пиріг з куркою", "Assets/Products/quiche.png", true, "Кіш з куркою", 92m },
                    { 46, 5, seedDate, "Солодка випічка з бананом", "Assets/Products/banana-bread.png", true, "Банановий хліб", 52m },
                    { 47, 6, seedDate, "Легкий мус з чорного шоколаду", "Assets/Products/mousse.png", true, "Шоколадний мус", 78m },
                    { 48, 1, seedDate, "Чорна кава ручного заварювання", "Assets/Products/filter-coffee.png", true, "Фільтр кава", 48m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("Products", "Id", 30);
            migrationBuilder.DeleteData("Products", "Id", 31);
            migrationBuilder.DeleteData("Products", "Id", 32);
            migrationBuilder.DeleteData("Products", "Id", 33);
            migrationBuilder.DeleteData("Products", "Id", 34);
            migrationBuilder.DeleteData("Products", "Id", 35);
            migrationBuilder.DeleteData("Products", "Id", 36);
            migrationBuilder.DeleteData("Products", "Id", 37);
            migrationBuilder.DeleteData("Products", "Id", 38);
            migrationBuilder.DeleteData("Products", "Id", 39);
            migrationBuilder.DeleteData("Products", "Id", 40);
            migrationBuilder.DeleteData("Products", "Id", 41);
            migrationBuilder.DeleteData("Products", "Id", 42);
            migrationBuilder.DeleteData("Products", "Id", 43);
            migrationBuilder.DeleteData("Products", "Id", 44);
            migrationBuilder.DeleteData("Products", "Id", 45);
            migrationBuilder.DeleteData("Products", "Id", 46);
            migrationBuilder.DeleteData("Products", "Id", 47);
            migrationBuilder.DeleteData("Products", "Id", 48);

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Products");
        }

        private static void SetImage(MigrationBuilder migrationBuilder, int id, string imageName)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: id,
                column: "ImagePath",
                value: $"Assets/Products/{imageName}.png");
        }
    }
}
