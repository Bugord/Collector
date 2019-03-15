using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class debtValueNullableMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Value",
                table: "Debts",
                nullable: true,
                oldClrType: typeof(float));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Value",
                table: "Debts",
                nullable: false,
                oldClrType: typeof(float),
                oldNullable: true);
        }
    }
}
