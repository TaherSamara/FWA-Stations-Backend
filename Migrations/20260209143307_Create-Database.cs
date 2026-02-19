using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    last_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    can_add_stations = table.Column<bool>(type: "bit", nullable: false),
                    can_edit_stations = table.Column<bool>(type: "bit", nullable: false),
                    can_delete_stations = table.Column<bool>(type: "bit", nullable: false),
                    can_add_subscribers = table.Column<bool>(type: "bit", nullable: false),
                    can_edit_subscribers = table.Column<bool>(type: "bit", nullable: false),
                    can_delete_subscribers = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    entity_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    entity_id = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ErrorLogs", x => x.id);
                    table.ForeignKey(
                        name: "FK_ErrorLogs_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ErrorLogs_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ErrorLogs_Users_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Stations", x => x.id);
                    table.ForeignKey(
                        name: "FK_Stations_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stations_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stations_Users_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    line_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    unit_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    link_mac_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    unit_direction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    management_ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mikrotik_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mikrotik_mac_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sas_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sas_port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    odf_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    odf_port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    management_vlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    service_type = table.Column<int>(type: "int", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    station_id = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Subscribers", x => x.id);
                    table.ForeignKey(
                        name: "FK_Subscribers_Stations_station_id",
                        column: x => x.station_id,
                        principalTable: "Stations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscribers_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscribers_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscribers_Users_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_delete_user_id",
                table: "ErrorLogs",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_insert_user_id",
                table: "ErrorLogs",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_update_user_id",
                table: "ErrorLogs",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_delete_user_id",
                table: "Stations",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_insert_user_id",
                table: "Stations",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_update_user_id",
                table: "Stations",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_delete_user_id",
                table: "Subscribers",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_insert_user_id",
                table: "Subscribers",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_station_id",
                table: "Subscribers",
                column: "station_id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_update_user_id",
                table: "Subscribers",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_delete_user_id",
                table: "Users",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_insert_user_id",
                table: "Users",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_update_user_id",
                table: "Users",
                column: "update_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
