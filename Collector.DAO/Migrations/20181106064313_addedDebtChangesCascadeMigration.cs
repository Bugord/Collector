using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class addedDebtChangesCascadeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Changes_Debts_ChangedDebtId",
                table: "Changes");

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_Debts_ChangedDebtId",
                table: "Changes",
                column: "ChangedDebtId",
                principalTable: "Debts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Changes_Debts_ChangedDebtId",
                table: "Changes");

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_Debts_ChangedDebtId",
                table: "Changes",
                column: "ChangedDebtId",
                principalTable: "Debts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
