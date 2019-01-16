using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class sentToNullableLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SentTo",
                table: "ChatMessages",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SentTo",
                table: "ChatMessages",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
