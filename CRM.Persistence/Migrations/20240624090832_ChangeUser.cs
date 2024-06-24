using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
  /// <inheritdoc />
  public partial class ChangeUser : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
        name: "RegistrationDate",
        table: "Users");

      migrationBuilder.AddColumn<DateTime>(
        name: "RegistrationDate",
        table: "Users",
        type: "timestamp with time zone",
        nullable: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
        name: "RegistrationDate",
        table: "Users");

      migrationBuilder.AddColumn<string>(
        name: "RegistrationDate",
        table: "Users",
        type: "text",
        nullable: false);
    }
  }
}
