using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class makeoptionalobservations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Observations",
                schema: "FlowManager",
                table: "Requests",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8000);

            migrationBuilder.AlterColumn<string>(
                name: "Observations",
                schema: "FlowManager",
                table: "RequestLogs",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8000);

            migrationBuilder.AlterColumn<string>(
                name: "Observations",
                schema: "FlowManager",
                table: "PreTrackingsStep",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8000);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Observations",
                schema: "FlowManager",
                table: "Requests",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observations",
                schema: "FlowManager",
                table: "RequestLogs",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observations",
                schema: "FlowManager",
                table: "PreTrackingsStep",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8000,
                oldNullable: true);
        }
    }
}
