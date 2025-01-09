using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace order_orderline.Migrations
{
    /// <inheritdoc />
    public partial class addproductnameonorderline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "OrderLines",
                keyColumn: "OrderLineId",
                keyValue: 1,
                column: "ProductName",
                value: null);

            migrationBuilder.UpdateData(
                table: "OrderLines",
                keyColumn: "OrderLineId",
                keyValue: 2,
                column: "ProductName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderDate",
                value: new DateOnly(2024, 12, 20));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2,
                column: "OrderDate",
                value: new DateOnly(2024, 12, 19));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderLines");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderDate",
                value: new DateOnly(2024, 12, 17));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2,
                column: "OrderDate",
                value: new DateOnly(2024, 12, 16));
        }
    }
}
