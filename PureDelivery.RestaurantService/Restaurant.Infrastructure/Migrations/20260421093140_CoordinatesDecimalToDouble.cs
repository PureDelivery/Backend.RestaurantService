using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CoordinatesDecimalToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "ZonePoints",
                type: "float(11)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "ZonePoints",
                type: "float(10)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,8)",
                oldPrecision: 10,
                oldScale: 8);

            migrationBuilder.AddColumn<decimal>(
                name: "LoyaltyPointsRate",
                table: "PartnershipInfos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Addresses",
                type: "float(11)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Addresses",
                type: "float(10)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,8)",
                oldPrecision: 10,
                oldScale: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoyaltyPointsRate",
                table: "PartnershipInfos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "ZonePoints",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(11)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "ZonePoints",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Addresses",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(11)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Addresses",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 8);
        }
    }
}
