﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationlaptop.Migrations
{
    /// <inheritdoc />
    public partial class addhotenordermodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hovaten",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hovaten",
                table: "Orders");
        }
    }
}