using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersPermitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "can_add_users",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_delete_users",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_edit_users",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "can_add_users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_delete_users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_edit_users",
                table: "Users");
        }
    }
}
