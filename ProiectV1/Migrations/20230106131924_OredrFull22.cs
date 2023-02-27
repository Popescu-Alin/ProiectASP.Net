using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectV1.Migrations
{
    public partial class OredrFull22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrder_Orders_OrderId",
                table: "ProductOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrder_Products_ProductId",
                table: "ProductOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOrder",
                table: "ProductOrder");

            migrationBuilder.RenameTable(
                name: "ProductOrder",
                newName: "ProductOrders");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOrder_OrderId",
                table: "ProductOrders",
                newName: "IX_ProductOrders_OrderId");

            migrationBuilder.AlterColumn<double>(
                name: "TotalSum",
                table: "Orders",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOrders",
                table: "ProductOrders",
                columns: new[] { "ProductId", "OrderId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrders_Orders_OrderId",
                table: "ProductOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrders_Products_ProductId",
                table: "ProductOrders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrders_Orders_OrderId",
                table: "ProductOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrders_Products_ProductId",
                table: "ProductOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOrders",
                table: "ProductOrders");

            migrationBuilder.RenameTable(
                name: "ProductOrders",
                newName: "ProductOrder");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOrders_OrderId",
                table: "ProductOrder",
                newName: "IX_ProductOrder_OrderId");

            migrationBuilder.AlterColumn<float>(
                name: "TotalSum",
                table: "Orders",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOrder",
                table: "ProductOrder",
                columns: new[] { "ProductId", "OrderId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrder_Orders_OrderId",
                table: "ProductOrder",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrder_Products_ProductId",
                table: "ProductOrder",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
