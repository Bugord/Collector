using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class paymentsDeniedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Denied",
                table: "Payments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Denied",
                table: "Payments");
        }
    }
}
