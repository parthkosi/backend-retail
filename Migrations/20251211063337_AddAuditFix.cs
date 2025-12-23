using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_PurchaseRequests_PrId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_ActionBy",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Users_OwnerId",
                table: "PurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_CreatedFromPrItemId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_OwnerId",
                table: "PurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ActionBy",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_PrId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ManagerComment",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "PurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "ActionComment",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AuditLogs",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "ActionType",
                table: "AuditLogs",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "ActionBy",
                table: "AuditLogs",
                newName: "PerformedBy");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PurchaseRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PrId",
                table: "AuditLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_CreatedFromPrItemId",
                table: "Stocks",
                column: "CreatedFromPrItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_CreatedFromPrItemId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "AuditLogs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "PerformedBy",
                table: "AuditLogs",
                newName: "ActionBy");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "AuditLogs",
                newName: "ActionType");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PurchaseRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ManagerComment",
                table: "PurchaseRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PurchaseRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "PurchaseRequestItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PrId",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionComment",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_CreatedFromPrItemId",
                table: "Stocks",
                column: "CreatedFromPrItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_OwnerId",
                table: "PurchaseRequests",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActionBy",
                table: "AuditLogs",
                column: "ActionBy");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PrId",
                table: "AuditLogs",
                column: "PrId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_PurchaseRequests_PrId",
                table: "AuditLogs",
                column: "PrId",
                principalTable: "PurchaseRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_ActionBy",
                table: "AuditLogs",
                column: "ActionBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Users_OwnerId",
                table: "PurchaseRequests",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
