using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class addsubstepcodetokeyindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubStepLogs",
                schema: "FlowManager",
                table: "SubStepLogs");

            migrationBuilder.DropIndex(
                name: "IX_SubStepLogs_TrackingId_StepId_Attempts",
                schema: "FlowManager",
                table: "SubStepLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubStepLogs",
                schema: "FlowManager",
                table: "SubStepLogs",
                columns: new[] { "TrackingId", "StepId", "SubStepCode", "Attempts" });

            migrationBuilder.CreateIndex(
                name: "IX_SubStepLogs_TrackingId_StepId_SubStepCode_Attempts",
                schema: "FlowManager",
                table: "SubStepLogs",
                columns: new[] { "TrackingId", "StepId", "SubStepCode", "Attempts" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubStepLogs",
                schema: "FlowManager",
                table: "SubStepLogs");

            migrationBuilder.DropIndex(
                name: "IX_SubStepLogs_TrackingId_StepId_SubStepCode_Attempts",
                schema: "FlowManager",
                table: "SubStepLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubStepLogs",
                schema: "FlowManager",
                table: "SubStepLogs",
                columns: new[] { "TrackingId", "StepId", "Attempts" });

            migrationBuilder.CreateIndex(
                name: "IX_SubStepLogs_TrackingId_StepId_Attempts",
                schema: "FlowManager",
                table: "SubStepLogs",
                columns: new[] { "TrackingId", "StepId", "Attempts" },
                unique: true);
        }
    }
}
