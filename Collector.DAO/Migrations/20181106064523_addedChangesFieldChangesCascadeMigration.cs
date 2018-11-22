using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class addedChangesFieldChangesCascadeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldChanges_Changes_ChangeId",
                table: "FieldChanges");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldChanges_Changes_ChangeId",
                table: "FieldChanges",
                column: "ChangeId",
                principalTable: "Changes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldChanges_Changes_ChangeId",
                table: "FieldChanges");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldChanges_Changes_ChangeId",
                table: "FieldChanges",
                column: "ChangeId",
                principalTable: "Changes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
