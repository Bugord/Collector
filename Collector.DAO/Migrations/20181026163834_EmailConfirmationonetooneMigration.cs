using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class EmailConfirmationonetooneMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "EmailConfirmations",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmations_UserId",
                table: "EmailConfirmations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmations_Users_UserId",
                table: "EmailConfirmations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmations_Users_UserId",
                table: "EmailConfirmations");

            migrationBuilder.DropIndex(
                name: "IX_EmailConfirmations_UserId",
                table: "EmailConfirmations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmailConfirmations");
        }
    }
}
