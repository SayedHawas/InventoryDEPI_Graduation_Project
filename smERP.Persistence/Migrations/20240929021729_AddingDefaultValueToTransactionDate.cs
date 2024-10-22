using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingDefaultValueToTransactionDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "SalesTransactions_Payments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 99, DateTimeKind.Utc).AddTicks(9173));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "SalesTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 90, DateTimeKind.Utc).AddTicks(7910));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "ProcurementTransactions_Payments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 89, DateTimeKind.Utc).AddTicks(4847));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "ProcurementTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 79, DateTimeKind.Utc).AddTicks(6691));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "AdjustmentTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 2, 17, 29, 353, DateTimeKind.Utc).AddTicks(2344),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 73, DateTimeKind.Utc).AddTicks(5851));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "SalesTransactions_Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 99, DateTimeKind.Utc).AddTicks(9173),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "SalesTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 90, DateTimeKind.Utc).AddTicks(7910),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "ProcurementTransactions_Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 89, DateTimeKind.Utc).AddTicks(4847),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "ProcurementTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 79, DateTimeKind.Utc).AddTicks(6691),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "AdjustmentTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 73, DateTimeKind.Utc).AddTicks(5851),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 2, 17, 29, 353, DateTimeKind.Utc).AddTicks(2344));
        }
    }
}
