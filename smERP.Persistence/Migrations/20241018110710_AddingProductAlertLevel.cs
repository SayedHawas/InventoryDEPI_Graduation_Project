using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingProductAlertLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultQuantityAlertLevel",
                table: "ProductInstances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BranchProductInstanceAlertLevel",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    AlertLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchProductInstanceAlertLevel", x => new { x.BranchId, x.ProductInstanceId });
                    table.ForeignKey(
                        name: "FK_BranchProductInstanceAlertLevel_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchProductInstanceAlertLevel_ProductInstances_ProductInstanceId",
                        column: x => x.ProductInstanceId,
                        principalTable: "ProductInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchProductInstanceAlertLevel_ProductInstanceId",
                table: "BranchProductInstanceAlertLevel",
                column: "ProductInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchProductInstanceAlertLevel");

            migrationBuilder.DropColumn(
                name: "DefaultQuantityAlertLevel",
                table: "ProductInstances");
        }
    }
}
