using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class FriendTableColumnFixed2Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Friends",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Friends",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
