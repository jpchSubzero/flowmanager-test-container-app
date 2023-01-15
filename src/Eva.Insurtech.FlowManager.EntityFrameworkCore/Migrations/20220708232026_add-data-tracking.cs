using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class adddatatracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpClient",
                schema: "FlowManager",
                table: "Trackings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WayCode",
                schema: "FlowManager",
                table: "Trackings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpClient",
                schema: "FlowManager",
                table: "Trackings");

            migrationBuilder.DropColumn(
                name: "WayCode",
                schema: "FlowManager",
                table: "Trackings");
        }
    }
}
