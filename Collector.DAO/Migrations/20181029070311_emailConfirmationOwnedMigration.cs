using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class emailConfirmationOwnedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isClosed",
                table: "Feedbacks",
                newName: "IsClosed");

            migrationBuilder.AddColumn<long>(
                name: "ClosedById",
                table: "Feedbacks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ClosedById",
                table: "Feedbacks",
                column: "ClosedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_ClosedById",
                table: "Feedbacks",
                column: "ClosedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_ClosedById",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_ClosedById",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "ClosedById",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "IsClosed",
                table: "Feedbacks",
                newName: "isClosed");
        }
    }
}
