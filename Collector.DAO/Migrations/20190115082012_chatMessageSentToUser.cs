using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class chatMessageSentToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SentTo",
                table: "ChatMessages",
                newName: "SentToId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SentToId",
                table: "ChatMessages",
                column: "SentToId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_SentToId",
                table: "ChatMessages",
                column: "SentToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_SentToId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_SentToId",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "SentToId",
                table: "ChatMessages",
                newName: "SentTo");
        }
    }
}
