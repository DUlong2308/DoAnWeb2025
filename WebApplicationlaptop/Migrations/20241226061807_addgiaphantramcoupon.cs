using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationlaptop.Migrations
{
    /// <inheritdoc />
    public partial class addgiaphantramcoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountPercentage",
                table: "Coupons",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "Pricederease",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "Pricederease",
                table: "Coupons");
        }
    }
}
