using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class AddIndexCodeToCatalogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Catalogs",
                schema: "FlowManager",
                table: "Catalogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Catalogs",
                schema: "FlowManager",
                table: "Catalogs",
                column: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Catalogs",
                schema: "FlowManager",
                table: "Catalogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Catalogs",
                schema: "FlowManager",
                table: "Catalogs",
                column: "Id");
        }
    }
}
