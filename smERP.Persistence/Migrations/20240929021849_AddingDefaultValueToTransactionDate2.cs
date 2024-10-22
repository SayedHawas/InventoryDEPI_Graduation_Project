using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingDefaultValueToTransactionDate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "AdjustmentTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 2, 18, 49, 3, DateTimeKind.Utc).AddTicks(2629),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 2, 17, 29, 353, DateTimeKind.Utc).AddTicks(2344));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "AdjustmentTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 2, 17, 29, 353, DateTimeKind.Utc).AddTicks(2344),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 2, 18, 49, 3, DateTimeKind.Utc).AddTicks(2629));
        }
    }
}
