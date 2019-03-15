using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class addedCurrenciesRecordsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO dbo.Currencies(CurrencyName, CurrencySymbol, Created, CreatedBy)" +
                " VALUES(\'Dollar\', \'$\',SYSUTCDATETIME(), 0)");

            migrationBuilder.Sql(
                "INSERT INTO dbo.Currencies(CurrencyName, CurrencySymbol, Created, CreatedBy)" +
                " VALUES(\'Russian ruble\', \'RUB\',SYSUTCDATETIME(), 0)");

            migrationBuilder.Sql(
                "INSERT INTO dbo.Currencies(CurrencyName, CurrencySymbol, Created, CreatedBy)" +
                " VALUES(\'Belarusian ruble\', \'BYN\',SYSUTCDATETIME(), 0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}