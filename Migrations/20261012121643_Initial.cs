using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantiy",
                table: "InvoiceItem",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "InvoiceDaate",
                table: "Invoice",
                newName: "InvoiceDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsCredit",
                table: "Invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCredit",
                table: "Invoice");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "InvoiceItem",
                newName: "Quantiy");

            migrationBuilder.RenameColumn(
                name: "InvoiceDate",
                table: "Invoice",
                newName: "InvoiceDaate");
        }
    }
}
