using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class addsubsteplogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubStepLogs",
                schema: "FlowManager",
                columns: table => new
                {
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    SubStepCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RegisterTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubStepLogs", x => new { x.TrackingId, x.StepId, x.Attempts });
                    table.ForeignKey(
                        name: "FK_SubStepLogs_Trackings_TrackingId",
                        column: x => x.TrackingId,
                        principalSchema: "FlowManager",
                        principalTable: "Trackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubStepLogs_TrackingId_StepId_Attempts",
                schema: "FlowManager",
                table: "SubStepLogs",
                columns: new[] { "TrackingId", "StepId", "Attempts" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubStepLogs",
                schema: "FlowManager");
        }
    }
}
