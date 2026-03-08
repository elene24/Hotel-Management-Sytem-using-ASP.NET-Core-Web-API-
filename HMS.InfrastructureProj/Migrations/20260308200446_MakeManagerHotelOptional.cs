using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.InfrastructureProj.Migrations
{
    /// <inheritdoc />
    public partial class MakeManagerHotelOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Hotels_HotelId",
                table: "Managers");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Managers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Hotels_HotelId",
                table: "Managers",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Hotels_HotelId",
                table: "Managers");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Managers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Hotels_HotelId",
                table: "Managers",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
