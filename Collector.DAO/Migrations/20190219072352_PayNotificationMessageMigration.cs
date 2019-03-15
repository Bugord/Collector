using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class PayNotificationMessageMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "PayNotifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "PayNotifications");
        }
    }
}
