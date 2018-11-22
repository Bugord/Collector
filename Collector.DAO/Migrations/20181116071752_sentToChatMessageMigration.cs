using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class sentToChatMessageMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SentTo",
                table: "ChatMessages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentTo",
                table: "ChatMessages");
        }
    }
}
