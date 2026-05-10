using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoffeeShopApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Кава" },
                    { 2, "Холодні напої" },
                    { 3, "Сніданки" },
                    { 4, "Сендвічі та перекуси" },
                    { 5, "Випічка" },
                    { 6, "Десерти" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Класичний міцний еспресо", true, "Еспресо", 25m },
                    { 2, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Подвійний еспресо", true, "Допіо", 35m },
                    { 3, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Еспресо з гарячою водою", true, "Американо", 30m },
                    { 4, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Еспресо з молоком та пінкою", true, "Капучино", 40m },
                    { 5, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ніжна кава з молоком", true, "Лате", 45m },
                    { 6, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Подвійний еспресо з молоком", true, "Флет Вайт", 50m },
                    { 7, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Кава з вершками та ваніллю", true, "Раф кава", 55m },
                    { 8, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Кава з шоколадом", true, "Мокачино", 55m },
                    { 9, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Кава холодного заварювання", true, "Колд брю", 55m },
                    { 10, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Колд брю з тоніком", true, "Колд брю тонік", 65m },
                    { 11, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Холодне лате з льодом", true, "Айс лате", 55m },
                    { 12, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Домашній лимонад", true, "Лимонад", 45m },
                    { 13, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Свіжовичавлений сік", true, "Апельсиновий фреш", 60m },
                    { 14, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Чорний або зелений чай", true, "Чай", 30m },
                    { 15, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Яєчня, бекон, тости", true, "Англійський сніданок", 120m },
                    { 16, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ніжний омлет з сиром", true, "Омлет з сиром", 85m },
                    { 17, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Сирники зі сметаною", true, "Сирники", 95m },
                    { 18, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Корисний сніданок", true, "Вівсянка з фруктами", 75m },
                    { 19, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Курка, сир, овочі", true, "Сендвіч з куркою", 85m },
                    { 20, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Теплий сендвіч", true, "Паніні з шинкою", 90m },
                    { 21, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Тост з авокадо та яйцем", true, "Тост з авокадо", 110m },
                    { 22, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Французький круасан", true, "Круасан", 35m },
                    { 23, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Круасан з шоколадною начинкою", true, "Круасан з шоколадом", 45m },
                    { 24, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ароматна булочка", true, "Булочка з корицею", 42m },
                    { 25, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Шоколадний маффін", true, "Маффін шоколадний", 38m },
                    { 26, 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Класичний чізкейк", true, "Чізкейк", 75m },
                    { 27, 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Італійський десерт", true, "Тірамісу", 80m },
                    { 28, 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Шоколадний десерт", true, "Брауні", 60m },
                    { 29, 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Домашній медовий торт", true, "Медівник", 70m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
