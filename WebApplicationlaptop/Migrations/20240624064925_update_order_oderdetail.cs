using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationlaptop.Migrations
{
    /// <inheritdoc />
    public partial class update_order_oderdetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderModelId1",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderModelId1",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderModelId1",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<int>(
                name: "OrderModelId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderModelId",
                table: "OrderDetails",
                column: "OrderModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderModelId",
                table: "OrderDetails",
                column: "OrderModelId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderModelId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderModelId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<long>(
                name: "OrderModelId",
                table: "OrderDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrderModelId1",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderModelId1",
                table: "OrderDetails",
                column: "OrderModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderModelId1",
                table: "OrderDetails",
                column: "OrderModelId1",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}