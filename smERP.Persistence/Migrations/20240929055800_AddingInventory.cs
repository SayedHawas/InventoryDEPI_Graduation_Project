using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShelfLifeInDays",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "AdjustmentTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 29, 2, 18, 49, 3, DateTimeKind.Utc).AddTicks(2629));

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name_English = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name_Arabic = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageLocations_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredProductInstance",
                columns: table => new
                {
                    StorageLocationId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredProductInstance", x => new { x.StorageLocationId, x.ProductInstanceId });
                    table.ForeignKey(
                        name: "FK_StoredProductInstance_ProductInstances_ProductInstanceId",
                        column: x => x.ProductInstanceId,
                        principalTable: "ProductInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoredProductInstance_StorageLocations_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductInstanceItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StorageLocationId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInstanceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductInstanceItem_StoredProductInstance_StorageLocationId_ProductInstanceId",
                        columns: x => new { x.StorageLocationId, x.ProductInstanceId },
                        principalTable: "StoredProductInstance",
                        principalColumns: new[] { "StorageLocationId", "ProductInstanceId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Name_Arabic",
                table: "Branches",
                column: "Name_Arabic")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Name_English",
                table: "Branches",
                column: "Name_English")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductInstanceItem_SerialNumber",
                table: "ProductInstanceItem",
                column: "SerialNumber")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductInstanceItem_StorageLocationId_ProductInstanceId",
                table: "ProductInstanceItem",
                columns: new[] { "StorageLocationId", "ProductInstanceId" });

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_BranchId",
                table: "StorageLocations",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredProductInstance_ProductInstanceId",
                table: "StoredProductInstance",
                column: "ProductInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductInstanceItem");

            migrationBuilder.DropTable(
                name: "StoredProductInstance");

            migrationBuilder.DropTable(
                name: "StorageLocations");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropColumn(
                name: "ShelfLifeInDays",
                table: "Products");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                table: "AdjustmentTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 29, 2, 18, 49, 3, DateTimeKind.Utc).AddTicks(2629),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
