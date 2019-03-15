using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class PayNotificationAddMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    PayerId = table.Column<long>(nullable: true),
                    DebtOwnerId = table.Column<long>(nullable: true),
                    DebtId = table.Column<long>(nullable: false),
                    Value = table.Column<float>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayNotifications_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayNotifications_Users_DebtOwnerId",
                        column: x => x.DebtOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayNotifications_Users_PayerId",
                        column: x => x.PayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayNotifications_DebtId",
                table: "PayNotifications",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_PayNotifications_DebtOwnerId",
                table: "PayNotifications",
                column: "DebtOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PayNotifications_PayerId",
                table: "PayNotifications",
                column: "PayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayNotifications");
        }
    }
}
