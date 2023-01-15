using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class AddProcessLogMaxTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxLifeTime",
                schema: "FlowManager",
                table: "FlowSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxLifeTime",
                schema: "FlowManager",
                table: "Flows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OtpMaxAttempts",
                schema: "FlowManager",
                table: "Flows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OtpMaxResends",
                schema: "FlowManager",
                table: "Flows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OtpMaxTime",
                schema: "FlowManager",
                table: "Flows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProcessLogs",
                schema: "FlowManager",
                columns: table => new
                {
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Request = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    RegisterTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessLogs", x => new { x.TrackingId, x.StepId, x.Version });
                    table.ForeignKey(
                        name: "FK_ProcessLogs_Trackings_TrackingId",
                        column: x => x.TrackingId,
                        principalSchema: "FlowManager",
                        principalTable: "Trackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessLogs_TrackingId_StepId_Version",
                schema: "FlowManager",
                table: "ProcessLogs",
                columns: new[] { "TrackingId", "StepId", "Version" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessLogs",
                schema: "FlowManager");

            migrationBuilder.DropColumn(
                name: "MaxLifeTime",
                schema: "FlowManager",
                table: "FlowSteps");

            migrationBuilder.DropColumn(
                name: "MaxLifeTime",
                schema: "FlowManager",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "OtpMaxAttempts",
                schema: "FlowManager",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "OtpMaxResends",
                schema: "FlowManager",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "OtpMaxTime",
                schema: "FlowManager",
                table: "Flows");
        }
    }
}
