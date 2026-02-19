using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseStructsher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "can_add_stations",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_add_subscribers",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_add_users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_delete_stations",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_delete_subscribers",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_delete_users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_edit_stations",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_edit_subscribers",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "can_edit_users",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    device_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    serial_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    source_location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    destination_location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_line_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    assigned_user_id = table.Column<int>(type: "int", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_user_id = table.Column<int>(type: "int", nullable: true),
                    update_user_id = table.Column<int>(type: "int", nullable: true),
                    delete_user_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.id);
                    table.ForeignKey(
                        name: "FK_Warehouse_Users_assigned_user_id",
                        column: x => x.assigned_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Warehouse_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warehouse_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warehouse_Users_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    permission_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "Permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_permission_id",
                table: "UserPermissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_user_id",
                table: "UserPermissions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_assigned_user_id",
                table: "Warehouse",
                column: "assigned_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_delete_user_id",
                table: "Warehouse",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_insert_user_id",
                table: "Warehouse",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_update_user_id",
                table: "Warehouse",
                column: "update_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.AddColumn<bool>(
                name: "can_add_stations",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_add_subscribers",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_add_users",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_delete_stations",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_delete_subscribers",
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
                name: "can_edit_stations",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_edit_subscribers",
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
    }
}
