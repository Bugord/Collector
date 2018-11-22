using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class emailConfirmationAddedUsedMigartion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConfirmedDate",
                table: "EmailConfirmations",
                newName: "ConfirmationTime");

            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "EmailConfirmations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Used",
                table: "EmailConfirmations");

            migrationBuilder.RenameColumn(
                name: "ConfirmationTime",
                table: "EmailConfirmations",
                newName: "ConfirmedDate");
        }
    }
}
