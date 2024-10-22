using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserAccountDisable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccountDisabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountDisabled",
                table: "Users");
        }
    }
}
