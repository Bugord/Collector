using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class feedbackClosedByUpdateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedById",
                table: "Feedbacks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClosedById",
                table: "Feedbacks",
                nullable: true);
        }
    }
}
