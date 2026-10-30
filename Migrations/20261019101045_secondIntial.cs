using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceManagement.Migrations
{
    /// <inheritdoc />
    public partial class secondIntial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "StockTransaction",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransaction_InvoiceId",
                table: "StockTransaction",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransaction_Invoice_InvoiceId",
                table: "StockTransaction",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTransaction_Invoice_InvoiceId",
                table: "StockTransaction");

            migrationBuilder.DropIndex(
                name: "IX_StockTransaction_InvoiceId",
                table: "StockTransaction");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "StockTransaction");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Invoice");
        }
    }
}
