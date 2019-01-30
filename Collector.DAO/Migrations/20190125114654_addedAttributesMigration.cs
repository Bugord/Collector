using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class addedAttributesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_AuthorId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackMessages_Users_AuthorId",
                table: "FeedbackMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackMessages_Feedbacks_FeedbackId",
                table: "FeedbackMessages");

            migrationBuilder.AlterColumn<string>(
                name: "FieldName",
                table: "FieldChanges",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ChangeId",
                table: "FieldChanges",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FeedbackId",
                table: "FeedbackMessages",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AuthorId",
                table: "FeedbackMessages",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AuthorId",
                table: "ChatMessages",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ChangedDebtId",
                table: "Changes",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_AuthorId",
                table: "ChatMessages",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackMessages_Users_AuthorId",
                table: "FeedbackMessages",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackMessages_Feedbacks_FeedbackId",
                table: "FeedbackMessages",
                column: "FeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_AuthorId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackMessages_Users_AuthorId",
                table: "FeedbackMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackMessages_Feedbacks_FeedbackId",
                table: "FeedbackMessages");

            migrationBuilder.AlterColumn<string>(
                name: "FieldName",
                table: "FieldChanges",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<long>(
                name: "ChangeId",
                table: "FieldChanges",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "FeedbackId",
                table: "FeedbackMessages",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "AuthorId",
                table: "FeedbackMessages",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "AuthorId",
                table: "ChatMessages",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "ChangedDebtId",
                table: "Changes",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_AuthorId",
                table: "ChatMessages",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackMessages_Users_AuthorId",
                table: "FeedbackMessages",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackMessages_Feedbacks_FeedbackId",
                table: "FeedbackMessages",
                column: "FeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
