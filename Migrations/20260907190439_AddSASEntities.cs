using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class AddSASEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SAS",
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
                name: "SASPorts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sas_port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sas_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wan_ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    management_ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    line_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location_node = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    connect_to = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contact_person = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    order_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    connection_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cpe_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cpe_serial_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    odf_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    odf_port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    management_vlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sas_id = table.Column<int>(type: "int", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SASPorts");

            migrationBuilder.DropTable(
                name: "SAS");
        }
    }
}
