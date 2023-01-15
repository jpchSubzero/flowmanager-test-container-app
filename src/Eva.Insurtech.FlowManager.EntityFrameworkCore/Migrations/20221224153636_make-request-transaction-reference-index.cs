using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class makerequesttransactionreferenceindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestLogId_Service",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "TransactionReference", "RequestLogId", "Service" });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestLogId",
                schema: "FlowManager",
                table: "Requests",
                column: "RequestLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TransactionReference_RequestLogId_Service",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "TransactionReference", "RequestLogId", "Service" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestLogId",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_TransactionReference_RequestLogId_Service",
                schema: "FlowManager",
                table: "Requests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "RequestLogId", "Service" });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestLogId_Service",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "RequestLogId", "Service" },
                unique: true);
        }
    }
}
