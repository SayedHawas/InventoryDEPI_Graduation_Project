using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingInventoryTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SupplierAddresses",
                table: "SupplierAddresses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupplierAddresses",
                table: "SupplierAddresses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AdjustmentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 73, DateTimeKind.Utc).AddTicks(5851)),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjustmentTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 79, DateTimeKind.Utc).AddTicks(6691)),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementTransactions_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 90, DateTimeKind.Utc).AddTicks(7910)),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdjustmentTransactions_Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjustmentTransactions_Items", x => new { x.TransactionId, x.Id });
                    table.ForeignKey(
                        name: "FK_AdjustmentTransactions_Items_AdjustmentTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "AdjustmentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdjustmentTransactions_Items_ProductInstances_ProductInstanceId",
                        column: x => x.ProductInstanceId,
                        principalTable: "ProductInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementTransactions_Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementTransactions_Items", x => new { x.TransactionId, x.Id });
                    table.ForeignKey(
                        name: "FK_ProcurementTransactions_Items_ProcurementTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "ProcurementTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcurementTransactions_Items_ProductInstances_ProductInstanceId",
                        column: x => x.ProductInstanceId,
                        principalTable: "ProductInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementTransactions_Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    PayedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 89, DateTimeKind.Utc).AddTicks(4847)),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementTransactions_Payments", x => new { x.TransactionId, x.Id });
                    table.ForeignKey(
                        name: "FK_ProcurementTransactions_Payments_ProcurementTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "ProcurementTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesTransactions_Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesTransactions_Items", x => new { x.TransactionId, x.Id });
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Items_ProductInstances_ProductInstanceId",
                        column: x => x.ProductInstanceId,
                        principalTable: "ProductInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Items_SalesTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "SalesTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesTransactions_Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    PayedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 9, 29, 0, 31, 7, 99, DateTimeKind.Utc).AddTicks(9173)),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesTransactions_Payments", x => new { x.TransactionId, x.Id });
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Payments_SalesTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "SalesTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierAddresses_SupplierId",
                table: "SupplierAddresses",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_AdjustmentTransactions_Items_ProductInstanceId",
                table: "AdjustmentTransactions_Items",
                column: "ProductInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementTransactions_SupplierId",
                table: "ProcurementTransactions",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementTransactions_Items_ProductInstanceId",
                table: "ProcurementTransactions_Items",
                column: "ProductInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_Items_ProductInstanceId",
                table: "SalesTransactions_Items",
                column: "ProductInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdjustmentTransactions_Items");

            migrationBuilder.DropTable(
                name: "ProcurementTransactions_Items");

            migrationBuilder.DropTable(
                name: "ProcurementTransactions_Payments");

            migrationBuilder.DropTable(
                name: "SalesTransactions_Items");

            migrationBuilder.DropTable(
                name: "SalesTransactions_Payments");

            migrationBuilder.DropTable(
                name: "AdjustmentTransactions");

            migrationBuilder.DropTable(
                name: "ProcurementTransactions");

            migrationBuilder.DropTable(
                name: "SalesTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupplierAddresses",
                table: "SupplierAddresses");

            migrationBuilder.DropIndex(
                name: "IX_SupplierAddresses_SupplierId",
                table: "SupplierAddresses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupplierAddresses",
                table: "SupplierAddresses",
                columns: new[] { "SupplierId", "Id" });
        }
    }
}
