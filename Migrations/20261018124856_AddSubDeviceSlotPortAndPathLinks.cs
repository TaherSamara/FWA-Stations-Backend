using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class AddSubDeviceSlotPortAndPathLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "port",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "slot",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PathLinks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    from_sub_device_id = table.Column<int>(type: "int", nullable: false),
                    to_sub_device_id = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PathLinks", x => x.id);
                    table.ForeignKey(
                        name: "FK_PathLinks_SubDevices_from_sub_device_id",
                        column: x => x.from_sub_device_id,
                        principalTable: "SubDevices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PathLinks_SubDevices_to_sub_device_id",
                        column: x => x.to_sub_device_id,
                        principalTable: "SubDevices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PathLinks_Users_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PathLinks_Users_insert_user_id",
                        column: x => x.insert_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PathLinks_Users_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PathLinkOdfs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order = table.Column<int>(type: "int", nullable: false),
                    path_link_id = table.Column<int>(type: "int", nullable: false),
                    odf_sub_device_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PathLinkOdfs", x => x.id);
                    table.ForeignKey(
                        name: "FK_PathLinkOdfs_PathLinks_path_link_id",
                        column: x => x.path_link_id,
                        principalTable: "PathLinks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PathLinkOdfs_SubDevices_odf_sub_device_id",
                        column: x => x.odf_sub_device_id,
                        principalTable: "SubDevices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PathLinkOdfs_odf_sub_device_id",
                table: "PathLinkOdfs",
                column: "odf_sub_device_id");

            migrationBuilder.CreateIndex(
                name: "IX_PathLinkOdfs_path_link_id_order",
                table: "PathLinkOdfs",
                columns: new[] { "path_link_id", "order" });

            migrationBuilder.CreateIndex(
                name: "IX_PathLinks_delete_user_id",
                table: "PathLinks",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_PathLinks_from_sub_device_id",
                table: "PathLinks",
                column: "from_sub_device_id");

            migrationBuilder.CreateIndex(
                name: "IX_PathLinks_insert_user_id",
                table: "PathLinks",
                column: "insert_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_PathLinks_to_sub_device_id",
                table: "PathLinks",
                column: "to_sub_device_id");

            migrationBuilder.CreateIndex(
                name: "IX_PathLinks_update_user_id",
                table: "PathLinks",
                column: "update_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PathLinkOdfs");

            migrationBuilder.DropTable(
                name: "PathLinks");

            migrationBuilder.DropColumn(
                name: "port",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "slot",
                table: "SubDevices");
        }
    }
}
