using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantReviews", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantReviews_CreatedAt",
                table: "RestaurantReviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantReviews_CustomerId",
                table: "RestaurantReviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantReviews_Rating",
                table: "RestaurantReviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantReviews_RestaurantId",
                table: "RestaurantReviews",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantReviews");
        }
    }
}
