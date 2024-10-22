using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingTransactionItemUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdjustmentTransactions_Items_InventoryTransactionItemUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionItemId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjustmentTransactions_Items_InventoryTransactionItemUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdjustmentTransactions_Items_InventoryTransactionItemUnits_AdjustmentTransactions_Items_TransactionId_TransactionItemId",
                        columns: x => new { x.TransactionId, x.TransactionItemId },
                        principalTable: "AdjustmentTransactions_Items",
                        principalColumns: new[] { "TransactionId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementTransactions_Items_InventoryTransactionItemUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionItemId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementTransactions_Items_InventoryTransactionItemUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementTransactions_Items_InventoryTransactionItemUnits_ProcurementTransactions_Items_TransactionId_TransactionItemId",
                        columns: x => new { x.TransactionId, x.TransactionItemId },
                        principalTable: "ProcurementTransactions_Items",
                        principalColumns: new[] { "TransactionId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesTransactions_Items_InventoryTransactionItemUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionItemId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesTransactions_Items_InventoryTransactionItemUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Items_InventoryTransactionItemUnits_SalesTransactions_Items_TransactionId_TransactionItemId",
                        columns: x => new { x.TransactionId, x.TransactionItemId },
                        principalTable: "SalesTransactions_Items",
                        principalColumns: new[] { "TransactionId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdjustmentTransactions_Items_InventoryTransactionItemUnits_TransactionId_TransactionItemId",
                table: "AdjustmentTransactions_Items_InventoryTransactionItemUnits",
                columns: new[] { "TransactionId", "TransactionItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementTransactions_Items_InventoryTransactionItemUnits_TransactionId_TransactionItemId",
                table: "ProcurementTransactions_Items_InventoryTransactionItemUnits",
                columns: new[] { "TransactionId", "TransactionItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_Items_InventoryTransactionItemUnits_TransactionId_TransactionItemId",
                table: "SalesTransactions_Items_InventoryTransactionItemUnits",
                columns: new[] { "TransactionId", "TransactionItemId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdjustmentTransactions_Items_InventoryTransactionItemUnits");

            migrationBuilder.DropTable(
                name: "ProcurementTransactions_Items_InventoryTransactionItemUnits");

            migrationBuilder.DropTable(
                name: "SalesTransactions_Items_InventoryTransactionItemUnits");
        }
    }
}
