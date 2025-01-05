using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationlaptop.Migrations
{
    /// <inheritdoc />
    public partial class cuponver2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Coupons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
