using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePermissionsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "description",
                table: "Permissions",
                newName: "order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "order",
                table: "Permissions",
                newName: "description");
        }
    }
}
