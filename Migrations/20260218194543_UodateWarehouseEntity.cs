using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class UodateWarehouseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "destination_location",
                table: "Warehouse");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "destination_location",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
