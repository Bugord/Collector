using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class AddedSystemUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF (NOT EXISTS(SELECT * FROM dbo.Users WHERE Username = \'system\'))" +
                " BEGIN" +
                " INSERT INTO dbo.Users(Username, Email, Created, Role, FirstName, LastName, CreatedBy, Password)" +
                " VALUES(\'system\', \'system@admin.com\',SYSUTCDATETIME(), 0, \'Sys\', \'tem\', 0, \'admin\')" +
                " END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
