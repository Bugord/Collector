using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class FriendTableColumnFixedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronized",
                table: "Friends",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Friends",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynchronized",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Friends");
        }
    }
}
