using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class addedUsersNameMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsersName",
                table: "Friends",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersName",
                table: "Friends");
        }
    }
}
