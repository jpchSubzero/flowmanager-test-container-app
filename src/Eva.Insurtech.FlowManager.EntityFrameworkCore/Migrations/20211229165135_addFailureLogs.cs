using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class addFailureLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FailureLogs",
                schema: "FlowManager",
                columns: table => new
                {
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Error = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailureLogs", x => new { x.TrackingId, x.RegisterTime });
                    table.ForeignKey(
                        name: "FK_FailureLogs_Trackings_TrackingId",
                        column: x => x.TrackingId,
                        principalSchema: "FlowManager",
                        principalTable: "Trackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FailureLogs_TrackingId_RegisterTime",
                schema: "FlowManager",
                table: "FailureLogs",
                columns: new[] { "TrackingId", "RegisterTime" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FailureLogs",
                schema: "FlowManager");
        }
    }
}
