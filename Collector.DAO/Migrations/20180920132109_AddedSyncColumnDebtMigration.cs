using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class AddedSyncColumnDebtMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Debts",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<bool>(
                name: "Synchronize",
                table: "Debts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Synchronize",
                table: "Debts");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Debts",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 16);
        }
    }
}
