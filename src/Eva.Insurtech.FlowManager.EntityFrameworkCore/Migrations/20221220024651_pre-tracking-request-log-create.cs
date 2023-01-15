using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class pretrackingrequestlogcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreTrackings",
                schema: "FlowManager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Identification = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CellPhone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreTrackings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestLogs",
                schema: "FlowManager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Service = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Iterations = table.Column<int>(type: "int", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreTrackingsStep",
                schema: "FlowManager",
                columns: table => new
                {
                    PreTrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Container = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Component = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    Iterations = table.Column<int>(type: "int", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreTrackingsStep", x => new { x.PreTrackingId, x.RegisterDate });
                    table.ForeignKey(
                        name: "FK_PreTrackingsStep_PreTrackings_PreTrackingId",
                        column: x => x.PreTrackingId,
                        principalSchema: "FlowManager",
                        principalTable: "PreTrackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                schema: "FlowManager",
                columns: table => new
                {
                    RequestLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Service = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => new { x.RequestLogId, x.RegisterDate });
                    table.ForeignKey(
                        name: "FK_Requests_RequestLogs_RequestLogId",
                        column: x => x.RequestLogId,
                        principalSchema: "FlowManager",
                        principalTable: "RequestLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreTrackings_Id",
                schema: "FlowManager",
                table: "PreTrackings",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreTrackingsStep_PreTrackingId_RegisterDate",
                schema: "FlowManager",
                table: "PreTrackingsStep",
                columns: new[] { "PreTrackingId", "RegisterDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_Id",
                schema: "FlowManager",
                table: "RequestLogs",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestLogId_RegisterDate",
                schema: "FlowManager",
                table: "Requests",
                columns: new[] { "RequestLogId", "RegisterDate" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreTrackingsStep",
                schema: "FlowManager");

            migrationBuilder.DropTable(
                name: "Requests",
                schema: "FlowManager");

            migrationBuilder.DropTable(
                name: "PreTrackings",
                schema: "FlowManager");

            migrationBuilder.DropTable(
                name: "RequestLogs",
                schema: "FlowManager");
        }
    }
}
