using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class feedbackClosedByIdUpdateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClosedById",
                table: "Feedbacks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedById",
                table: "Feedbacks");
        }
    }
}
