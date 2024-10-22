using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingStorageLocationIdToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StorageLocationId",
                table: "SalesTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StorageLocationId",
                table: "ProcurementTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StorageLocationId",
                table: "AdjustmentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_StorageLocationId",
                table: "SalesTransactions",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementTransactions_StorageLocationId",
                table: "ProcurementTransactions",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdjustmentTransactions_StorageLocationId",
                table: "AdjustmentTransactions",
                column: "StorageLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdjustmentTransactions_StorageLocations_StorageLocationId",
                table: "AdjustmentTransactions",
                column: "StorageLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcurementTransactions_StorageLocations_StorageLocationId",
                table: "ProcurementTransactions",
                column: "StorageLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesTransactions_StorageLocations_StorageLocationId",
                table: "SalesTransactions",
                column: "StorageLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdjustmentTransactions_StorageLocations_StorageLocationId",
                table: "AdjustmentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcurementTransactions_StorageLocations_StorageLocationId",
                table: "ProcurementTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesTransactions_StorageLocations_StorageLocationId",
                table: "SalesTransactions");

            migrationBuilder.DropIndex(
                name: "IX_SalesTransactions_StorageLocationId",
                table: "SalesTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProcurementTransactions_StorageLocationId",
                table: "ProcurementTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AdjustmentTransactions_StorageLocationId",
                table: "AdjustmentTransactions");

            migrationBuilder.DropColumn(
                name: "StorageLocationId",
                table: "SalesTransactions");

            migrationBuilder.DropColumn(
                name: "StorageLocationId",
                table: "ProcurementTransactions");

            migrationBuilder.DropColumn(
                name: "StorageLocationId",
                table: "AdjustmentTransactions");
        }
    }
}
