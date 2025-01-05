using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationlaptop.Migrations
{
    /// <inheritdoc />
    public partial class updateagainproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop Foreign Key Constraint
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderModelId",
                table: "OrderDetails");

            // Step 2: Drop Primary Key Constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK__Orders__3214EC07BE40EDF1",
                table: "Orders");

            // Step 3: Alter Column Type
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Orders",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "OrderModelId",
                table: "OrderDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Step 4: Recreate Primary Key Constraint
            migrationBuilder.AddPrimaryKey(
                name: "PK__Orders__3214EC07BE40EDF1",
                table: "Orders",
                column: "Id");

            // Step 5: Recreate Foreign Key Constraint
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
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "OrderModelId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
