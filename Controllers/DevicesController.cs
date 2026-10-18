using ClosedXML.Excel;
using FWA_Stations.Entities;
using FWA_Stations.Models;
using FWA_Stations.ViewModels.Devices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FWA_Stations.Models.Enums;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class DevicesController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [Authorize(Roles = "VIEW_DEVICES")]
    [HttpGet("list")]
    public async Task<IActionResult> List(string q, int size = 20, int page = 1, int type = 0)
    {
        IQueryable<Devices> query = db.Devices.AsNoTracking()
            .Include(x => x.insert_user)
            .Include(x => x.update_user)
            .Where(x => (int)x.type == type);

        vm.total_count = await query.CountAsync();

        if (!string.IsNullOrEmpty(q))
        {
            string pattern = $"%{q.Trim()}%";
            query = query.Where(x => EF.Functions.Like(x.name, pattern));
        }

        vm.data = query
            .OrderByDescending(x => x.id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsEnumerable()
            .Select(x => new
            {
                x.id,
                x.name,
                x.type,
                x.insert_date,
                x.update_date,
                insert_user = GetUserDetails(x.insert_user),
                update_user = GetUserDetails(x.update_user)
            })
            .ToList();

        vm.total_records = await query.CountAsync();

        return Ok(new SuccessResponse(true, "Data Returned Successfully.", vm));
    }

    [Authorize(Roles = "ADD_DEVICE")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] DevicesVM model)
    {
        var device = await db.Devices.AnyAsync(x => x.name == model.name);

        if (device)
            return Ok(new SuccessResponse(false, "Device Already Exists!"));

        var newDevice = new Devices
        {
            name = model.name,
            type = (DeviceType)model.type,
            insert_user_id = GetMyID(),
            insert_date = DateTime.UtcNow
        };

        await db.Devices.AddAsync(newDevice);
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Device Added Successfully."));
    }

    [Authorize(Roles = "EDIT_DEVICE")]
    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] DevicesVM model)
    {
        var device = db.Devices.FirstOrDefault(x => x.id == id);

        if (device is null)
            return Ok(new SuccessResponse(false, "Device Not Exist!"));

        device.name = model.name;
        device.type = (DeviceType)model.type;
        device.update_user_id = GetMyID();
        device.update_date = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Device Updated Successfully."));
    }

    [Authorize(Roles = "DELETE_DEVICE")]
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var device = db.Devices.FirstOrDefault(x => x.id == id);

        if (device is null)
            return Ok(new SuccessResponse(false, "Device Not Exist!"));

        device.is_delete = true;
        device.delete_user_id = GetMyID();
        device.delete_date = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Device Deleted Successfully."));
    }

    [Authorize(Roles = "IMPORT_DEVICES")]
    [HttpPost("import")]
    public async Task<IActionResult> Import([FromQuery] int device_type, [FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
            return Ok(new SuccessResponse(false, "Please choose an Excel file."));

        if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            return Ok(new SuccessResponse(false, "Only .xlsx Excel files are allowed."));

        if (!Enum.IsDefined(typeof(DeviceType), device_type))
            return Ok(new SuccessResponse(false, "Invalid device type!"));

        var selectedDeviceType = (DeviceType)device_type;

        try
        {
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            if (!workbook.Worksheets.Any())
                return Ok(new SuccessResponse(false, "The Excel file does not contain any worksheets."));

            var worksheet = workbook.Worksheet(1);

            string[] requiredHeaders =
            [
                "device_name",
                "slot",
                "port",
                "name",
                "station_name",
                "line_code",
                "order_number",
                "unit_type",
                "unit_direction",
                "link_mac_address",
                "connection_type",
                "speed",
                "management_ip",
                "management_vlan",
                "wan_ip",
                "mikrotik_id",
                "mikrotik_mac_address",
                "sas_port",
                "odf_name",
                "odf_port",
                "customer_address",
                "contact_person",
                "contact_person_phone_no",
                "other_details"
            ];

            for (int column = 1; column <= requiredHeaders.Length; column++)
            {
                string actualHeader = worksheet.Cell(1, column).GetString().Trim();
                string expectedHeader = requiredHeaders[column - 1];

                if (!string.Equals(actualHeader, expectedHeader, StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new SuccessResponse(
                        false,
                        $"Invalid column at position {column}. Expected '{expectedHeader}' but found '{actualHeader}'."));
                }
            }

            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            if (lastRow < 2)
                return Ok(new SuccessResponse(false, "The Excel file does not contain any data."));

            var importRows = new List<SubDeviceImportRow>();
            var errors = new List<string>();
            var importedLineCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int row = 2; row <= lastRow; row++)
            {
                string deviceName = GetCellValue(worksheet, row, 1);
                string slot = GetCellValue(worksheet, row, 2);
                string port = GetCellValue(worksheet, row, 3);
                string name = GetCellValue(worksheet, row, 4);
                string stationName = GetCellValue(worksheet, row, 5);
                string lineCode = GetCellValue(worksheet, row, 6);
                string orderNumber = GetCellValue(worksheet, row, 7);
                string unitType = GetCellValue(worksheet, row, 8);
                string unitDirection = GetCellValue(worksheet, row, 9);
                string linkMacAddress = GetCellValue(worksheet, row, 10);
                string connectionType = GetCellValue(worksheet, row, 11);
                string speed = GetCellValue(worksheet, row, 12);
                string managementIp = GetCellValue(worksheet, row, 13);
                string managementVlan = GetCellValue(worksheet, row, 14);
                string wanIp = GetCellValue(worksheet, row, 15);
                string mikrotikId = GetCellValue(worksheet, row, 16);
                string mikrotikMacAddress = GetCellValue(worksheet, row, 17);
                string sasPort = GetCellValue(worksheet, row, 18);
                string odfName = GetCellValue(worksheet, row, 19);
                string odfPort = GetCellValue(worksheet, row, 20);
                string customerAddress = GetCellValue(worksheet, row, 21);
                string contactPerson = GetCellValue(worksheet, row, 22);
                string contactPersonPhoneNo = GetCellValue(worksheet, row, 23);
                string otherDetails = GetCellValue(worksheet, row, 24);

                bool isEmptyRow =
                    string.IsNullOrWhiteSpace(deviceName) &&
                    string.IsNullOrWhiteSpace(slot) &&
                    string.IsNullOrWhiteSpace(port) &&
                    string.IsNullOrWhiteSpace(name) &&
                    string.IsNullOrWhiteSpace(stationName) &&
                    string.IsNullOrWhiteSpace(lineCode) &&
                    string.IsNullOrWhiteSpace(orderNumber) &&
                    string.IsNullOrWhiteSpace(unitType) &&
                    string.IsNullOrWhiteSpace(managementIp) &&
                    string.IsNullOrWhiteSpace(wanIp);

                if (isEmptyRow)
                    continue;

                if (string.IsNullOrWhiteSpace(deviceName))
                    errors.Add($"Device name is required at row {row}.");

                if (string.IsNullOrWhiteSpace(name))
                    errors.Add($"Name is required at row {row}.");

                if (string.IsNullOrWhiteSpace(lineCode))
                {
                    errors.Add($"Line code is required at row {row}.");
                }
                else if (!importedLineCodes.Add(lineCode))
                {
                    errors.Add($"Line code '{lineCode}' is duplicated in the Excel file at row {row}.");
                }

                importRows.Add(new SubDeviceImportRow
                {
                    row_number = row,
                    device_name = deviceName,
                    slot = slot,
                    port = port,
                    name = name,
                    station_name = stationName,
                    line_code = lineCode,
                    order_number = orderNumber,
                    unit_type = unitType,
                    unit_direction = unitDirection,
                    link_mac_address = linkMacAddress,
                    connection_type = connectionType,
                    speed = speed,
                    management_ip = managementIp,
                    management_vlan = managementVlan,
                    wan_ip = wanIp,
                    mikrotik_id = mikrotikId,
                    mikrotik_mac_address = mikrotikMacAddress,
                    sas_port = sasPort,
                    odf_name = odfName,
                    odf_port = odfPort,
                    customer_address = customerAddress,
                    contact_person = contactPerson,
                    contact_person_phone_no = contactPersonPhoneNo,
                    other_details = otherDetails
                });
            }

            if (errors.Count > 0)
                return Ok(new SuccessResponse(false, "The Excel file contains invalid data.", new { errors }));

            if (importRows.Count == 0)
                return Ok(new SuccessResponse(false, "The Excel file does not contain any valid data rows."));

            int currentUserId = GetMyID();
            DateTime currentDate = DateTime.UtcNow;

            using var transaction = await db.Database.BeginTransactionAsync();

            var deviceNames = importRows
                .Select(x => x.device_name)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingDevices = await db.Devices
                .Where(x => x.type == selectedDeviceType && deviceNames.Contains(x.name))
                .ToListAsync();

            var deviceByName = existingDevices
                .GroupBy(x => x.name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderByDescending(y => y.id).First(),
                    StringComparer.OrdinalIgnoreCase);

            int createdDevicesCount = 0;

            foreach (var deviceName in deviceNames)
            {
                if (deviceByName.ContainsKey(deviceName))
                    continue;

                var newDevice = new Devices
                {
                    name = deviceName,
                    type = selectedDeviceType,
                    insert_user_id = currentUserId,
                    insert_date = currentDate
                };

                await db.Devices.AddAsync(newDevice);
                deviceByName[deviceName] = newDevice;
                createdDevicesCount++;
            }

            await db.SaveChangesAsync();

            var lineCodes = importRows
                .Select(x => x.line_code)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingRecords = await db.SubDevices
                .Where(x => lineCodes.Contains(x.line_code))
                .ToListAsync();

            var existingByLineCode = existingRecords
                .GroupBy(x => x.line_code, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderByDescending(y => y.id).First(),
                    StringComparer.OrdinalIgnoreCase);

            int insertedCount = 0;
            int updatedCount = 0;

            foreach (var row in importRows)
            {
                int deviceId = deviceByName[row.device_name].id;

                if (existingByLineCode.TryGetValue(row.line_code, out var existingSubDevice))
                {
                    ApplyImportValues(existingSubDevice, row);

                    existingSubDevice.device_id = deviceId;
                    existingSubDevice.update_user_id = currentUserId;
                    existingSubDevice.update_date = currentDate;

                    updatedCount++;
                    continue;
                }

                var newSubDevice = new SubDevices
                {
                    device_id = deviceId,
                    insert_user_id = currentUserId,
                    insert_date = currentDate
                };

                ApplyImportValues(newSubDevice, row);

                await db.SubDevices.AddAsync(newSubDevice);

                existingByLineCode[row.line_code] = newSubDevice;
                insertedCount++;
            }

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new SuccessResponse(
                true,
                "Sub Devices imported successfully.",
                new
                {
                    created_devices_count = createdDevicesCount,
                    inserted_count = insertedCount,
                    updated_count = updatedCount,
                    total_count = insertedCount + updatedCount
                }));
        }
        catch (DbUpdateException)
        {
            return Ok(new SuccessResponse(false, "A database error occurred while importing the Sub Devices."));
        }
        catch (Exception ex)
        {
            return Ok(new SuccessResponse(false, $"Error importing file: {ex.Message}"));
        }
    }

    [Authorize(Roles = "EXPORT_DEVICES")]
    [HttpGet("export/{device_id}")]
    public async Task<IActionResult> Export(int device_id)
    {
        var device = await db.Devices.AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == device_id);

        if (device is null)
            return Ok(new SuccessResponse(false, "Device Not Found!"));

        var subDevices = await db.SubDevices.AsNoTracking()
            .Where(x => x.device_id == device_id)
            .OrderBy(x => x.slot)
            .ThenBy(x => x.port)
            .ThenBy(x => x.name)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sub Devices");

        string[] headers =
        [
            "device_name",
            "slot",
            "port",
            "name",
            "station_name",
            "line_code",
            "order_number",
            "unit_type",
            "unit_direction",
            "link_mac_address",
            "connection_type",
            "speed",
            "management_ip",
            "management_vlan",
            "wan_ip",
            "mikrotik_id",
            "mikrotik_mac_address",
            "sas_port",
            "odf_name",
            "odf_port",
            "customer_address",
            "contact_person",
            "contact_person_phone_no",
            "other_details"
        ];

        for (int column = 1; column <= headers.Length; column++)
            worksheet.Cell(1, column).Value = headers[column - 1];

        var headerRange = worksheet.Range(1, 1, 1, headers.Length);

        headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(118, 188, 33);
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontSize = 11;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Alignment.WrapText = true;

        headerRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;

        headerRange.Style.Border.TopBorderColor = XLColor.FromArgb(118, 188, 33);
        headerRange.Style.Border.BottomBorderColor = XLColor.FromArgb(118, 188, 33);
        headerRange.Style.Border.LeftBorderColor = XLColor.FromArgb(118, 188, 33);
        headerRange.Style.Border.RightBorderColor = XLColor.FromArgb(118, 188, 33);

        worksheet.Row(1).Height = 26;

        int row = 2;

        foreach (var subDevice in subDevices)
        {
            int column = 1;

            worksheet.Cell(row, column++).Value = device.name ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.slot ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.port ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.name ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.station_name ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.line_code ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.order_number ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.unit_type ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.unit_direction ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.link_mac_address ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.connection_type ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.speed ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.management_ip ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.management_vlan ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.wan_ip ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.mikrotik_id ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.mikrotik_mac_address ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.sas_port ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.odf_name ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.odf_port ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.customer_address ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.contact_person ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.contact_person_phone_no ?? string.Empty;
            worksheet.Cell(row, column++).Value = subDevice.other_details ?? string.Empty;

            row++;
        }

        if (subDevices.Count > 0)
        {
            int lastDataRow = row - 1;

            var dataRange = worksheet.Range(2, 1, lastDataRow, headers.Length);

            dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorderColor = XLColor.FromArgb(220, 220, 220);
            dataRange.Style.Border.OutsideBorderColor = XLColor.FromArgb(190, 190, 190);

            for (int dataRow = 2; dataRow <= lastDataRow; dataRow++)
            {
                if (dataRow % 2 == 0)
                {
                    worksheet.Range(dataRow, 1, dataRow, headers.Length)
                        .Style.Fill.BackgroundColor = XLColor.FromArgb(247, 250, 244);
                }
            }

            int[] textColumns = [2, 3, 6, 7, 10, 12, 13, 14, 15, 16, 17, 18, 20, 23];

            foreach (int textColumn in textColumns)
                worksheet.Range(2, textColumn, lastDataRow, textColumn).Style.NumberFormat.Format = "@";
        }

        worksheet.SheetView.FreezeRows(1);

        int tableLastRow = Math.Max(1, row - 1);
        worksheet.Range(1, 1, tableLastRow, headers.Length).SetAutoFilter();

        worksheet.Columns(1, headers.Length).AdjustToContents();

        foreach (var column in worksheet.Columns(1, headers.Length))
        {
            if (column.Width < 12)
                column.Width = 12;

            if (column.Width > 35)
                column.Width = 35;
        }

        worksheet.Column(21).Width = 30;
        worksheet.Column(24).Width = 35;

        worksheet.Column(21).Style.Alignment.WrapText = true;
        worksheet.Column(24).Style.Alignment.WrapText = true;

        worksheet.RowsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        string safeDeviceName = string.Join(
            "_",
            device.name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

        string fileName = $"{safeDeviceName}_SubDevices_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    private static string GetCellValue(IXLWorksheet worksheet, int row, int column)
    {
        return worksheet.Cell(row, column).GetFormattedString().Trim();
    }

    private static void ApplyImportValues(SubDevices subDevice, SubDeviceImportRow row)
    {
        subDevice.slot = row.slot;
        subDevice.port = row.port;
        subDevice.name = row.name;
        subDevice.station_name = row.station_name;
        subDevice.line_code = row.line_code;
        subDevice.order_number = row.order_number;
        subDevice.unit_type = row.unit_type;
        subDevice.unit_direction = row.unit_direction;
        subDevice.link_mac_address = row.link_mac_address;
        subDevice.connection_type = row.connection_type;
        subDevice.speed = row.speed;
        subDevice.management_ip = row.management_ip;
        subDevice.management_vlan = row.management_vlan;
        subDevice.wan_ip = row.wan_ip;
        subDevice.mikrotik_id = row.mikrotik_id;
        subDevice.mikrotik_mac_address = row.mikrotik_mac_address;
        subDevice.sas_port = row.sas_port;
        subDevice.odf_name = row.odf_name;
        subDevice.odf_port = row.odf_port;
        subDevice.customer_address = row.customer_address;
        subDevice.contact_person = row.contact_person;
        subDevice.contact_person_phone_no = row.contact_person_phone_no;
        subDevice.other_details = row.other_details;
    }

    private sealed class SubDeviceImportRow
    {
        public int row_number { get; init; }

        public string device_name { get; init; }
        public string slot { get; init; }
        public string port { get; init; }
        public string name { get; init; }
        public string station_name { get; init; }
        public string line_code { get; init; }
        public string order_number { get; init; }
        public string unit_type { get; init; }
        public string unit_direction { get; init; }
        public string link_mac_address { get; init; }
        public string connection_type { get; init; }
        public string speed { get; init; }
        public string management_ip { get; init; }
        public string management_vlan { get; init; }
        public string wan_ip { get; init; }
        public string mikrotik_id { get; init; }
        public string mikrotik_mac_address { get; init; }
        public string sas_port { get; init; }
        public string odf_name { get; init; }
        public string odf_port { get; init; }
        public string customer_address { get; init; }
        public string contact_person { get; init; }
        public string contact_person_phone_no { get; init; }
        public string other_details { get; init; }
    }
}