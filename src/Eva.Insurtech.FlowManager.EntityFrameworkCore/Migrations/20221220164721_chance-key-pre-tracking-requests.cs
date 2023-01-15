using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class chancekeypretrackingrequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestLogId_RegisterDate",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PreTrackingsStep",
                schema: "FlowManager",
                table: "PreTrackingsStep");

            migrationBuilder.DropIndex(
                name: "IX_PreTrackingsStep_PreTrackingId_RegisterDate",
                schema: "FlowManager",
                table: "PreTrackingsStep");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "RequestLogId", "Service" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PreTrackingsStep",
                schema: "FlowManager",
                table: "PreTrackingsStep",
                columns: new[] { "PreTrackingId", "Container", "Component", "Method" });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestLogId_Service",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "RequestLogId", "Service" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreTrackingsStep_PreTrackingId_Container_Component_Method",
                schema: "FlowManager",
                table: "PreTrackingsStep",
                columns: new[] { "PreTrackingId", "Container", "Component", "Method" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestLogId_Service",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PreTrackingsStep",
                schema: "FlowManager",
                table: "PreTrackingsStep");

            migrationBuilder.DropIndex(
                name: "IX_PreTrackingsStep_PreTrackingId_Container_Component_Method",
                schema: "FlowManager",
                table: "PreTrackingsStep");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "RequestLogId", "RegisterDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PreTrackingsStep",
                schema: "FlowManager",
                table: "PreTrackingsStep",
                columns: new[] { "PreTrackingId", "RegisterDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestLogId_RegisterDate",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "RequestLogId", "RegisterDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreTrackingsStep_PreTrackingId_RegisterDate",
                schema: "FlowManager",
                table: "PreTrackingsStep",
                columns: new[] { "PreTrackingId", "RegisterDate" },
                unique: true);
        }
    }
}
