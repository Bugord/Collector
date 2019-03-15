using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class PayNotificationUserToPayRenameMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayNotifications_Users_DebtOwnerId",
                table: "PayNotifications");

            migrationBuilder.RenameColumn(
                name: "DebtOwnerId",
                table: "PayNotifications",
                newName: "UserToPayId");

            migrationBuilder.RenameIndex(
                name: "IX_PayNotifications_DebtOwnerId",
                table: "PayNotifications",
                newName: "IX_PayNotifications_UserToPayId");

            migrationBuilder.AddForeignKey(
                name: "FK_PayNotifications_Users_UserToPayId",
                table: "PayNotifications",
                column: "UserToPayId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayNotifications_Users_UserToPayId",
                table: "PayNotifications");

            migrationBuilder.RenameColumn(
                name: "UserToPayId",
                table: "PayNotifications",
                newName: "DebtOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_PayNotifications_UserToPayId",
                table: "PayNotifications",
                newName: "IX_PayNotifications_DebtOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PayNotifications_Users_DebtOwnerId",
                table: "PayNotifications",
                column: "DebtOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
