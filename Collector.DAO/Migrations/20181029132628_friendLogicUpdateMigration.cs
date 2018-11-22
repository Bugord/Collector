using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class friendLogicUpdateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersName",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Friends",
                newName: "FriendUserId");

            migrationBuilder.RenameColumn(
                name: "OwnersName",
                table: "Friends",
                newName: "Name");

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "Friends",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendUserId",
                table: "Friends",
                column: "FriendUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_OwnerId",
                table: "Friends",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_FriendId",
                table: "Debts",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_OwnerId",
                table: "Debts",
                column: "OwnerId");

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
                name: "FK_Friends_Users_FriendUserId",
                table: "Friends",
                column: "FriendUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Users_OwnerId",
                table: "Friends",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
                name: "FK_Friends_Users_FriendUserId",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Users_OwnerId",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_FriendUserId",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_OwnerId",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Debts_FriendId",
                table: "Debts");

            migrationBuilder.DropIndex(
                name: "IX_Debts_OwnerId",
                table: "Debts");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Friends",
                newName: "OwnersName");

            migrationBuilder.RenameColumn(
                name: "FriendUserId",
                table: "Friends",
                newName: "UserId");

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "Friends",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsersName",
                table: "Friends",
                maxLength: 100,
                nullable: true);
        }
    }
}
