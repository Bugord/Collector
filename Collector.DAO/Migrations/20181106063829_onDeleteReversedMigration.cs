using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class onDeleteReversedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Friends_FriendId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Users_OwnerId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmations_Users_UserId",
                table: "EmailConfirmations");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResets_Users_UserId",
                table: "PasswordResets");

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Friends_FriendId",
                table: "Debts",
                column: "FriendId",
                principalTable: "Friends",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Users_OwnerId",
                table: "Debts",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmations_Users_UserId",
                table: "EmailConfirmations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResets_Users_UserId",
                table: "PasswordResets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Friends_FriendId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Users_OwnerId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmations_Users_UserId",
                table: "EmailConfirmations");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResets_Users_UserId",
                table: "PasswordResets");

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Friends_FriendId",
                table: "Debts",
                column: "FriendId",
                principalTable: "Friends",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Users_OwnerId",
                table: "Debts",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmations_Users_UserId",
                table: "EmailConfirmations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResets_Users_UserId",
                table: "PasswordResets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
