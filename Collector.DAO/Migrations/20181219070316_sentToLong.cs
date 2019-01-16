using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class sentToLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SentTo",
                table: "ChatMessages",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SentTo",
                table: "ChatMessages",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
