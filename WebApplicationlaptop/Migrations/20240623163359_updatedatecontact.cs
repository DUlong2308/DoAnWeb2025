using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationlaptop.Migrations
{
    /// <inheritdoc />
    public partial class updatedatecontact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Ngaygui",
                table: "Contacts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ngaygui",
                table: "Contacts");
        }
    }
}