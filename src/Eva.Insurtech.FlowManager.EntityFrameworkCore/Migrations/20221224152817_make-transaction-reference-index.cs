using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class maketransactionreferenceindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PreTrackings_Id",
                schema: "FlowManager",
                table: "PreTrackings");

            migrationBuilder.CreateIndex(
                name: "IX_PreTrackings_TransactionReference",
                schema: "FlowManager",
                table: "PreTrackings",
                column: "TransactionReference",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PreTrackings_TransactionReference",
                schema: "FlowManager",
                table: "PreTrackings");

            migrationBuilder.CreateIndex(
                name: "IX_PreTrackings_Id",
                schema: "FlowManager",
                table: "PreTrackings",
                column: "Id",
                unique: true);
        }
    }
}
