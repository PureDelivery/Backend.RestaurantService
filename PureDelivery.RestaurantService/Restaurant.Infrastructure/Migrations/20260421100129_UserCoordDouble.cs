using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserCoordDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "ZonePoints",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(11)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "ZonePoints",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 8);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Addresses",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(11)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Addresses",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "ZonePoints",
                type: "float(11)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "ZonePoints",
                type: "float(10)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Addresses",
                type: "float(11)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Addresses",
                type: "float(10)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
