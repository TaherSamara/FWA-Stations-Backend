using ClosedXML.Excel;
using FWA_Stations.Entities;
using FWA_Stations.Models;
using FWA_Stations.ViewModels.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class WarehouseController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [Authorize(Roles = "VIEW_ALL_DEVICES, VIEW_MY_DEVICES")]
    [HttpGet("list")]
    public async Task<IActionResult> List(string q, int size = 20, int page = 1)
    {
        IQueryable<Warehouse> query = db.Warehouse.AsNoTracking()
            .Include(x => x.assigned_user)
            .Include(x => x.insert_user)
            .Include(x => x.update_user);

        if (!CanView("VIEW_ALL_DEVICES"))
        {
            query = query.Where(x => x.assigned_user_id == GetMyID());
        }

        vm.total_count = await query.CountAsync();

        if (!string.IsNullOrEmpty(q))
        {
            string pattern = $"%{q.Trim()}%";
            query = query.Where(x => EF.Functions.Like(x.device_type, pattern)
                || EF.Functions.Like(x.serial_number, pattern)
                || EF.Functions.Like(x.customer_line_code, pattern)
                || EF.Functions.Like(x.assigned_user.full_name, pattern));
        }

        vm.data = query
            .OrderByDescending(x => x.id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsEnumerable()
            .Select(x => new
            {
                x.id,
                x.device_type,
                x.serial_number,
                x.source_location,
                x.customer_line_code,
                x.notes,
                x.insert_date,
                x.update_date,
                assigned_user = GetUserDetails(x.assigned_user),
                insert_user = GetUserDetails(x.insert_user),
                update_user = GetUserDetails(x.update_user)
            }).ToList();

        vm.total_records = await query.CountAsync();

        return Ok(new SuccessResponse(true, "Data Returned", vm));
    }


    [Authorize(Roles = "ADD_DEVICE")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] WarehouseVM model)
    {
        var deviceExists = await db.Warehouse.AnyAsync(x => x.serial_number == model.serial_number);

        if (deviceExists)
            return Ok(new SuccessResponse(false, "Device with this Serial Number Already Exists"));

        var newDevice = new Warehouse
        {
            device_type = model.device_type?.Trim() ?? "",
            serial_number = model.serial_number?.Trim() ?? "",
            source_location = model.source_location?.Trim() ?? "",
            customer_line_code = model.customer_line_code?.Trim() ?? "",
            notes = model.notes?.Trim() ?? "",
            assigned_user_id = model.assigned_user_id,
            insert_user_id = GetMyID(),
            insert_date = DateTime.UtcNow
        };

        await db.Warehouse.AddAsync(newDevice);
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Device Added Successfully"));
    }


    [Authorize(Roles = "EDIT_DEVICE")]
    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] WarehouseVM model)
    {
        var device = await db.Warehouse.FirstOrDefaultAsync(x => x.id == id);

        if (device is null)
            return Ok(new SuccessResponse(false, "Device Not Exist"));

        // Check if serial number already exists for another device
        var serialExists = await db.Warehouse.AnyAsync(x => x.serial_number == model.serial_number && x.id != id);
        if (serialExists)
            return Ok(new SuccessResponse(false, "Serial Number Already Exists"));

        device.device_type = model.device_type?.Trim() ?? "";
        device.serial_number = model.serial_number?.Trim() ?? "";
        device.source_location = model.source_location?.Trim() ?? "";
        device.customer_line_code = model.customer_line_code?.Trim() ?? "";
        device.notes = model.notes?.Trim() ?? "";
        device.assigned_user_id = model.assigned_user_id;
        device.update_user_id = GetMyID();
        device.update_date = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Device Updated Successfully"));
    }


    [Authorize(Roles = "UPDATE_CUSTOMER_LINE_CODE")]
    [HttpPost("update-customer-line-code/{id}")]
    public async Task<IActionResult> UpdateCustomerLineCode(int id, [FromBody] UpdateCustomerLineCodeVM model)
    {
        var device = await db.Warehouse.FirstOrDefaultAsync(x => x.id == id);

        if (device is null)
            return Ok(new SuccessResponse(false, "Device Not Exist"));

        // Update only customer line code and notes
        device.customer_line_code = model.customer_line_code?.Trim() ?? "";
        device.notes = model.notes?.Trim() ?? "";

        // Auto-assign user
        device.assigned_user_id = GetMyID();
        device.update_user_id = GetMyID();
        device.update_date = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Customer Line Code Updated Successfully"));
    }


    [Authorize(Roles = "DELETE_DEVICE")]
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var device = await db.Warehouse.FirstOrDefaultAsync(x => x.id == id);

        if (device is null)
            return Ok(new SuccessResponse(false, "Device Not Exist"));

        device.is_delete = true;
        device.delete_user_id = GetMyID();
        device.delete_date = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Device Deleted Successfully"));
    }


    [Authorize(Roles = "IMPORT_DEVICES")]
    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        try
        {
            if (file == null)
                return Ok(new SuccessResponse(false, "Please choose an Excel file with .xlsx format."));

            var extension = Path.GetExtension(file.FileName);
            if (extension.ToLower() != ".xlsx")
                return Ok(new SuccessResponse(false, "Only Excel files with .xlsx format are allowed."));

            int processedCount = 0;
            int addedCount = 0;
            int updatedCount = 0;

            using (var workbook = new XLWorkbook(file.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1);
                int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;

                if (rowCount < 2)
                    return Ok(new SuccessResponse(false, "Excel file is empty or has no data rows."));

                for (int row = 2; row <= rowCount; row++)
                {
                    var deviceType = worksheet.Cell(row, 1).GetValue<string>()?.Trim();
                    var serialNumber = worksheet.Cell(row, 2).GetValue<string>()?.Trim();
                    DateTime? parsedDateIn = worksheet.Cell(row, 3).GetValue<DateTime?>();
                    var sourceLocation = worksheet.Cell(row, 4).GetValue<string>()?.Trim();
                    var destinationLocation = worksheet.Cell(row, 5).GetValue<string>()?.Trim();

                    // Validate required fields
                    if (string.IsNullOrEmpty(deviceType))
                        return Ok(new SuccessResponse(false, $"Device type is required at row {row}."));

                    // Find assigned user by destination location (user name)
                    int? assignedUserId = null;
                    if (!string.IsNullOrEmpty(destinationLocation))
                    {
                        var users = await db.Users.ToListAsync();

                        var user = users.FirstOrDefault(x =>
                            $"{x.first_name} {x.last_name}".Trim().Equals(destinationLocation, StringComparison.OrdinalIgnoreCase) ||
                            x.email.Equals(destinationLocation, StringComparison.OrdinalIgnoreCase));

                        if (user == null)
                            return Ok(new SuccessResponse(false, $"Row {row}: User '{destinationLocation}' not found. Please check the user name."));

                        assignedUserId = user.id;
                    }

                    // Check if device exists
                    var existingDevice = await db.Warehouse
                        .FirstOrDefaultAsync(x => x.serial_number == serialNumber);

                    if (existingDevice != null)
                    {
                        // Update existing device
                        existingDevice.device_type = deviceType;
                        existingDevice.source_location = sourceLocation;
                        existingDevice.assigned_user_id = assignedUserId;
                        existingDevice.update_user_id = GetMyID();
                        existingDevice.update_date = DateTime.UtcNow;

                        updatedCount++;
                    }
                    else
                    {
                        // Add new device
                        var newDevice = new Warehouse
                        {
                            device_type = deviceType,
                            serial_number = serialNumber,
                            source_location = sourceLocation,
                            assigned_user_id = assignedUserId,
                            insert_user_id = GetMyID(),
                            insert_date = parsedDateIn,
                            is_delete = false
                        };

                        await db.Warehouse.AddAsync(newDevice);
                        addedCount++;
                    }

                    processedCount++;
                }
            }

            await db.SaveChangesAsync();

            var message = $"File imported successfully. Total: {processedCount}, Added: {addedCount}, Updated: {updatedCount}";
            return Ok(new SuccessResponse(true, message));
        }
        catch (Exception ex)
        {
            return Ok(new SuccessResponse(false, $"Error importing file: {ex.Message}"));
        }
    }
}
