using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SASPorts");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "SAS");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Devices", x => x.id);
                    table.ForeignKey(
                        name: "FK_Devices_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devices_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devices_Users_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubDevices",
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
                    device_id = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SubDevices", x => x.id);
                    table.ForeignKey(
                        name: "FK_SubDevices_Devices_device_id",
                        column: x => x.device_id,
                        principalTable: "Devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubDevices_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubDevices_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubDevices_Users_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_delete_user_id",
                table: "Devices",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_insert_user_id",
                table: "Devices",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_update_user_id",
                table: "Devices",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubDevices_delete_user_id",
                table: "SubDevices",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubDevices_device_id",
                table: "SubDevices",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubDevices_insert_user_id",
                table: "SubDevices",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubDevices_update_user_id",
                table: "SubDevices",
                column: "update_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubDevices");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.CreateTable(
                name: "SAS",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    delete_user_id = table.Column<int>(type: "int", nullable: true),
                    insert_user_id = table.Column<int>(type: "int", nullable: true),
                    update_user_id = table.Column<int>(type: "int", nullable: true),
                    delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAS", x => x.id);
                    table.ForeignKey(
                        name: "FK_SAS_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SAS_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SAS_Users_update_user_id",
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
                    delete_user_id = table.Column<int>(type: "int", nullable: true),
                    insert_user_id = table.Column<int>(type: "int", nullable: true),
                    update_user_id = table.Column<int>(type: "int", nullable: true),
                    delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "SASPorts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    delete_user_id = table.Column<int>(type: "int", nullable: true),
                    insert_user_id = table.Column<int>(type: "int", nullable: true),
                    sas_id = table.Column<int>(type: "int", nullable: false),
                    update_user_id = table.Column<int>(type: "int", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    connect_to = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    connection_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contact_person = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cpe_serial_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cpe_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: false),
                    line_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location_node = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    management_ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    management_vlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    odf_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    odf_port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    order_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sas_port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sas_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wan_ip = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SASPorts", x => x.id);
                    table.ForeignKey(
                        name: "FK_SASPorts_SAS_sas_id",
                        column: x => x.sas_id,
                        principalTable: "SAS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SASPorts_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SASPorts_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SASPorts_Users_update_user_id",
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
                    delete_user_id = table.Column<int>(type: "int", nullable: true),
                    insert_user_id = table.Column<int>(type: "int", nullable: true),
                    station_id = table.Column<int>(type: "int", nullable: false),
                    update_user_id = table.Column<int>(type: "int", nullable: true),
                    delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: false),
                    line_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    link_mac_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    management_ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    management_vlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mikrotik_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mikrotik_mac_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    odf_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    odf_port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sas_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sas_port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    service_type = table.Column<int>(type: "int", nullable: false),
                    unit_direction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    unit_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "IX_SAS_delete_user_id",
                table: "SAS",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SAS_insert_user_id",
                table: "SAS",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SAS_update_user_id",
                table: "SAS",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SASPorts_delete_user_id",
                table: "SASPorts",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SASPorts_insert_user_id",
                table: "SASPorts",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SASPorts_sas_id",
                table: "SASPorts",
                column: "sas_id");

            migrationBuilder.CreateIndex(
                name: "IX_SASPorts_update_user_id",
                table: "SASPorts",
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
        }
    }
}
