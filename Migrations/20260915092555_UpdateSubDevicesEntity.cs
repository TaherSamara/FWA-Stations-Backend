using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWA_Stations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubDevicesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "connection_type",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_person",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_person_phone_no",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_address",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "order_number",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "other_details",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "speed",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "station_name",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wan_ip",
                table: "SubDevices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "connection_type",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "contact_person",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "contact_person_phone_no",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "customer_address",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "order_number",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "other_details",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "speed",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "station_name",
                table: "SubDevices");

            migrationBuilder.DropColumn(
                name: "wan_ip",
                table: "SubDevices");
        }
    }
}
