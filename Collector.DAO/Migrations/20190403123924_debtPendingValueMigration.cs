using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class debtPendingValueMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "PendingValue",
                table: "Debts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingValue",
                table: "Debts");
        }
    }
}
