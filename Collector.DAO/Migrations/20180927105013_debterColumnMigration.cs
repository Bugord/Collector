using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class debterColumnMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOwnerDebter",
                table: "Debts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOwnerDebter",
                table: "Debts");
        }
    }
}
