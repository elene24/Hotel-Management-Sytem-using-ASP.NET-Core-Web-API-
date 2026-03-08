using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.InfrastructureProj.Migrations
{
    /// <inheritdoc />
    public partial class AuthUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Managers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Managers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Guests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Guests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Guests");
        }
    }
}
