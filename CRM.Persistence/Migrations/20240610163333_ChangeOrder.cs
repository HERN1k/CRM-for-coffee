using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderIssueDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WorkerChackoutId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "WorkerKitchenId",
                table: "Orders",
                newName: "WorkerId");

            migrationBuilder.RenameColumn(
                name: "OrderReceiptDate",
                table: "Orders",
                newName: "OrderСreationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "Orders",
                newName: "WorkerKitchenId");

            migrationBuilder.RenameColumn(
                name: "OrderСreationDate",
                table: "Orders",
                newName: "OrderReceiptDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderIssueDate",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkerChackoutId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
