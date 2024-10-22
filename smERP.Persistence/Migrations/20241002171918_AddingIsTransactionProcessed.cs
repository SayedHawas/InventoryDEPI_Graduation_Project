using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingIsTransactionProcessed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTransactionProcessed",
                table: "SalesTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTransactionProcessed",
                table: "ProcurementTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTransactionProcessed",
                table: "AdjustmentTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTransactionProcessed",
                table: "SalesTransactions");

            migrationBuilder.DropColumn(
                name: "IsTransactionProcessed",
                table: "ProcurementTransactions");

            migrationBuilder.DropColumn(
                name: "IsTransactionProcessed",
                table: "AdjustmentTransactions");
        }
    }
}
