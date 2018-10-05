using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class addedValueFieldDebtMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "Debts",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Debts");
        }
    }
}
