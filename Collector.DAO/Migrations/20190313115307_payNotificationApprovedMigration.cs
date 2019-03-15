using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class payNotificationApprovedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "PayNotifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "PayNotifications");
        }
    }
}
