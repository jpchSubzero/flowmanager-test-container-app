using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eva.Insurtech.FlowManager.Migrations
{
    public partial class AuditLogEva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogEva",
                schema: "FlowManager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 32, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    DateTimeExecution = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_AuditLogEva", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEva_Id",
                schema: "FlowManager",
                table: "AuditLogEva",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEva_TrackingId",
                schema: "FlowManager",
                table: "AuditLogEva",
                column: "TrackingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogEva",
                schema: "FlowManager");
        }
    }
}
