using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class paymentStatusMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Denied",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Payments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payments");

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Payments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Denied",
                table: "Payments",
                nullable: false,
                defaultValue: false);
        }
    }
}
