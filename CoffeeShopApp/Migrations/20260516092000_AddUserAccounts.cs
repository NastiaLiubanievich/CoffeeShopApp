using CoffeeShopApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopApp.Migrations;

[DbContext(typeof(CoffeeShopDbContext))]
[Migration("20260516092000_AddUserAccounts")]
public partial class AddUserAccounts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.Sql("""
            SET IDENTITY_INSERT Users ON;

            INSERT INTO Users (Id, FullName, Username, Password, Role, IsActive)
            VALUES
            (1, N'Адміністратор', N'admin', N'admin123', N'Адміністратор', 1),
            (2, N'Касир', N'cashier', N'cashier123', N'Касир', 1),
            (3, N'Менеджер', N'manager', N'manager123', N'Менеджер', 1);

            SET IDENTITY_INSERT Users OFF;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Users");
    }
}
