using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class friendDeletedParamsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Friends");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Friends",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Friends",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Friends",
                maxLength: 16,
                nullable: true);
        }
    }
}
