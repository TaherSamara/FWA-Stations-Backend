using ClosedXML.Excel;
using FWA_Stations.Entities;
using FWA_Stations.Models;
using FWA_Stations.ViewModels.Subscribers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static FWA_Stations.Models.Enums;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class SubscribersController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [Authorize(Roles = "VIEW_SUBSCRIBERS")]
    [HttpGet("list")]
    public async Task<IActionResult> List(string q, int size = 20, int page = 1, int? stationId = null, string serviceTypes = null)
    {
        IQueryable<Subscribers> query = db.Subscribers.AsNoTracking()
            .Include(x => x.insert_user)
            .Include(x => x.update_user)
            .Include(x => x.station);

        vm.total_count = await query.CountAsync();

        if (!string.IsNullOrEmpty(q))
        {
            string pattern = $"%{q.Trim()}%";
            query = query.Where(x => EF.Functions.Like(x.name, pattern)
                || EF.Functions.Like(x.line_code, pattern)
                || EF.Functions.Like(x.management_ip, pattern));
        }

        if (stationId.HasValue)
        {
            query = query.Where(x => x.station_id == stationId.Value);
        }

        // Filter by service types
        if (!string.IsNullOrEmpty(serviceTypes))
        {
            var serviceTypeList = serviceTypes.Split(',')
                .Select(s => int.TryParse(s.Trim(), out var value) ? (ServiceType?)value : null)
                .Where(s => s.HasValue)
                .Select(s => s.Value)
                .ToList();

            if (serviceTypeList.Any())
            {
                query = query.Where(x => serviceTypeList.Contains(x.service_type));
            }
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
                x.line_code,
                x.unit_type,
                x.link_mac_address,
                x.unit_direction,
                x.management_ip,
                x.mikrotik_id,
                x.mikrotik_mac_address,
                x.sas_name,
                x.sas_port,
                x.odf_name,
                x.odf_port,
                x.management_vlan,
                x.service_type,
                x.notes,
                x.insert_date,
                x.update_date,
                station = GetStationDetails(x.station),
                insert_user = GetUserDetails(x.insert_user),
                update_user = GetUserDetails(x.update_user)
            }).ToList();

        vm.total_records = await query.CountAsync();

        return Ok(new SuccessResponse(true, "Data Returned", vm));
    }


    [Authorize(Roles = "ADD_SUBSCRIBER")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] SubscribersVM model)
    {
        var subscriber = await db.Subscribers.AnyAsync(x => x.name == model.name && x.station_id == model.station_id);

        if (subscriber)
            return Ok(new SuccessResponse(false, "Subscriber Already Exists"));

        var station = await db.Stations.AnyAsync(x => x.id == model.station_id);

        if (!station)
            return Ok(new SuccessResponse(false, "Station Not Found"));

        var newSubscriber = new Subscribers
        {
            name = model.name,
            line_code = model.line_code,
            unit_type = model.unit_type,
            link_mac_address = model.link_mac_address,
            unit_direction = model.unit_direction,
            management_ip = model.management_ip,
            mikrotik_id = model.mikrotik_id,
            mikrotik_mac_address = model.mikrotik_mac_address,
            sas_name = model.sas_name,
            sas_port = model.sas_port,
            odf_name = model.odf_name,
            odf_port = model.odf_port,
            management_vlan = model.management_vlan,
            service_type = model.service_type,
            notes = model.notes,
            station_id = model.station_id,
            insert_user_id = GetMyID(),
            insert_date = DateTime.UtcNow
        };

        await db.Subscribers.AddAsync(newSubscriber);
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Subscriber Added Successfully"));
    }


    [Authorize(Roles = "EDIT_SUBSCRIBER")]
    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] SubscribersVM model)
    {
        var subscriber = db.Subscribers.FirstOrDefault(x => x.id == id);

        if (subscriber is null)
            return Ok(new SuccessResponse(false, "Subscriber Not Exist"));

        var station = await db.Stations.AnyAsync(x => x.id == model.station_id);

        if (!station)
            return Ok(new SuccessResponse(false, "Station Not Found"));

        subscriber.name = model.name;
        subscriber.line_code = model.line_code;
        subscriber.unit_type = model.unit_type;
        subscriber.link_mac_address = model.link_mac_address;
        subscriber.unit_direction = model.unit_direction;
        subscriber.management_ip = model.management_ip;
        subscriber.mikrotik_id = model.mikrotik_id;
        subscriber.mikrotik_mac_address = model.mikrotik_mac_address;
        subscriber.sas_name = model.sas_name;
        subscriber.sas_port = model.sas_port;
        subscriber.odf_name = model.odf_name;
        subscriber.odf_port = model.odf_port;
        subscriber.management_vlan = model.management_vlan;
        subscriber.service_type = model.service_type;
        subscriber.notes = model.notes;
        subscriber.station_id = model.station_id;
        subscriber.update_user_id = GetMyID();
        subscriber.update_date = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Subscriber Updated Successfully"));
    }


    [Authorize(Roles = "DELETE_SUBSCRIBER")]
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var subscriber = db.Subscribers.FirstOrDefault(x => x.id == id);

        if (subscriber is null)
            return Ok(new SuccessResponse(false, "Subscriber Not Exist"));

        subscriber.is_delete = true;
        subscriber.delete_user_id = GetMyID();
        subscriber.delete_date = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Station Deleted Successfully"));
    }


    [HttpPost("ping/{ip}")]
    public async Task<IActionResult> Ping(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return Ok(new SuccessResponse(false, "IP is required!"));

        if (!System.Net.IPAddress.TryParse(ip, out _))
            return Ok(new SuccessResponse(false, "Invalid IP address format!"));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var startInfo = new ProcessStartInfo
        {
            FileName = "ping",
            Arguments = $"-n 4 -w 1000 {ip}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };

        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await Task.WhenAll(outputTask, errorTask);
        await process.WaitForExitAsync();

        stopwatch.Stop();

        var pingOutput = outputTask.Result;
        var errorOutput = errorTask.Result;

        var exitCode = process.ExitCode;

        bool pingable = exitCode == 0 &&
                        !pingOutput.Contains("Request timed out", StringComparison.OrdinalIgnoreCase);

        // Extract Min / Avg / Max
        string min = null, avg = null, max = null;
        var timeMatch = Regex.Match(
            pingOutput,
            @"Minimum = (\d+)ms, Maximum = (\d+)ms, Average = (\d+)ms",
            RegexOptions.IgnoreCase
        );

        if (timeMatch.Success)
        {
            min = timeMatch.Groups[1].Value;
            max = timeMatch.Groups[2].Value;
            avg = timeMatch.Groups[3].Value;
        }

        // Extract Packet Info
        int? sent = null, received = null, lost = null, lossPercent = null;

        var packetMatch = Regex.Match(
            pingOutput,
            @"Sent = (\d+), Received = (\d+), Lost = (\d+) \((\d+)% loss\)",
            RegexOptions.IgnoreCase
        );

        if (packetMatch.Success)
        {
            sent = int.Parse(packetMatch.Groups[1].Value);
            received = int.Parse(packetMatch.Groups[2].Value);
            lost = int.Parse(packetMatch.Groups[3].Value);
            lossPercent = int.Parse(packetMatch.Groups[4].Value);
        }

        // Extract TTL (first occurrence)
        string ttl = null;
        var ttlMatch = Regex.Match(pingOutput, @"TTL=(\d+)", RegexOptions.IgnoreCase);
        if (ttlMatch.Success)
            ttl = ttlMatch.Groups[1].Value;

        var data = new
        {
            ip,
            execution_time_ms = stopwatch.ElapsedMilliseconds,
            exit_code = exitCode,
            ping = new
            {
                status = pingable ? "Pingable" : "Not Reachable",
                statistics = new
                {
                    sent,
                    received,
                    lost,
                    loss_percent = lossPercent
                },
                latency_ms = new
                {
                    minimum = min,
                    average = avg,
                    maximum = max
                },
                ttl,
                raw_output = pingOutput,
                error = string.IsNullOrWhiteSpace(errorOutput) ? null : errorOutput
            }
        };

        return Ok(new SuccessResponse(true, "Ping data returned", data));
    }


    [Authorize(Roles = "IMPORT_SUBSCRIBERS")]
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

            var subscribersToProcess = new List<(Subscribers subscriber, bool isUpdate)>();

            using (var workbook = new XLWorkbook(file.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1);
                int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;

                if (rowCount < 2)
                    return Ok(new SuccessResponse(false, "Excel file is empty or has no data rows."));

                for (int row = 2; row <= rowCount; row++)
                {
                    var lineCode = worksheet.Cell(row, 1).GetValue<string>()?.Trim();
                    var subscriberName = worksheet.Cell(row, 2).GetValue<string>()?.Trim();
                    var unitType = worksheet.Cell(row, 3).GetValue<string>()?.Trim();
                    var linkMacAddress = worksheet.Cell(row, 4).GetValue<string>()?.Trim();
                    var unitDirection = worksheet.Cell(row, 5).GetValue<string>()?.Trim();
                    var managementIp = worksheet.Cell(row, 6).GetValue<string>()?.Trim();
                    var mikrotikId = worksheet.Cell(row, 7).GetValue<string>()?.Trim();
                    var mikrotikMacAddress = worksheet.Cell(row, 8).GetValue<string>()?.Trim();
                    var sasName = worksheet.Cell(row, 9).GetValue<string>()?.Trim();
                    var sasPort = worksheet.Cell(row, 10).GetValue<string>()?.Trim();
                    var odfName = worksheet.Cell(row, 11).GetValue<string>()?.Trim();
                    var odfPort = worksheet.Cell(row, 12).GetValue<string>()?.Trim();
                    var managementVlan = worksheet.Cell(row, 13).GetValue<string>()?.Trim();
                    var stationName = worksheet.Cell(row, 14).GetValue<string>()?.Trim();
                    var serviceTypeStr = worksheet.Cell(row, 15).GetValue<string>()?.Trim();
                    var notes = worksheet.Cell(row, 16).GetValue<string>()?.Trim();

                    // Skip empty rows
                    if (string.IsNullOrEmpty(lineCode) && string.IsNullOrEmpty(subscriberName))
                        continue;

                    // Validate required fields
                    if (string.IsNullOrEmpty(subscriberName))
                        return Ok(new SuccessResponse(false, $"Subscriber name is required at row {row}."));

                    if (string.IsNullOrEmpty(stationName))
                        return Ok(new SuccessResponse(false, $"Station name is required at row {row}."));

                    // Find or Create Station
                    var station = await db.Stations.FirstOrDefaultAsync(x => x.name == stationName);
                    if (station == null)
                    {
                        // Create new station
                        station = new Stations
                        {
                            name = stationName,
                            insert_user_id = GetMyID(),
                            insert_date = DateTime.UtcNow
                        };
                        await db.Stations.AddAsync(station);
                        await db.SaveChangesAsync(); // Save to get the station ID
                    }

                    // Parse Service Type
                    ServiceType serviceType = ServiceType.Mobadara;
                    if (!string.IsNullOrEmpty(serviceTypeStr))
                    {
                        if (serviceTypeStr.ToLower().Contains("mobadara"))
                            serviceType = ServiceType.Mobadara;
                        else if (serviceTypeStr.ToLower().Contains("ptp"))
                            serviceType = ServiceType.PTP;
                        else if (serviceTypeStr.ToLower().Contains("bs"))
                            serviceType = ServiceType.BaseStation;
                    }

                    // Check if subscriber exists by line_code + station_id
                    Subscribers existingSubscriber = null;

                    if (!string.IsNullOrEmpty(lineCode))
                    {
                        existingSubscriber = await db.Subscribers
                            .FirstOrDefaultAsync(x => x.line_code == lineCode && x.station_id == station.id);
                    }

                    if (existingSubscriber != null)
                    {
                        // Update existing subscriber
                        existingSubscriber.name = subscriberName;
                        existingSubscriber.line_code = lineCode;
                        existingSubscriber.unit_type = unitType;
                        existingSubscriber.link_mac_address = linkMacAddress;
                        existingSubscriber.unit_direction = unitDirection;
                        existingSubscriber.management_ip = managementIp;
                        existingSubscriber.mikrotik_id = mikrotikId;
                        existingSubscriber.mikrotik_mac_address = mikrotikMacAddress;
                        existingSubscriber.sas_name = sasName;
                        existingSubscriber.sas_port = sasPort;
                        existingSubscriber.odf_name = odfName;
                        existingSubscriber.odf_port = odfPort;
                        existingSubscriber.management_vlan = managementVlan;
                        existingSubscriber.service_type = serviceType;
                        existingSubscriber.notes = notes;
                        existingSubscriber.station_id = station.id;
                        existingSubscriber.update_user_id = GetMyID();
                        existingSubscriber.update_date = DateTime.UtcNow;

                        subscribersToProcess.Add((existingSubscriber, true));
                    }
                    else
                    {
                        // Add new subscriber
                        var newSubscriber = new Subscribers
                        {
                            name = subscriberName,
                            line_code = lineCode,
                            unit_type = unitType,
                            link_mac_address = linkMacAddress,
                            unit_direction = unitDirection,
                            management_ip = managementIp,
                            mikrotik_id = mikrotikId,
                            mikrotik_mac_address = mikrotikMacAddress,
                            sas_name = sasName,
                            sas_port = sasPort,
                            odf_name = odfName,
                            odf_port = odfPort,
                            management_vlan = managementVlan,
                            service_type = serviceType,
                            notes = notes,
                            station_id = station.id,
                            insert_user_id = GetMyID(),
                            insert_date = DateTime.UtcNow
                        };

                        await db.Subscribers.AddAsync(newSubscriber);
                        subscribersToProcess.Add((newSubscriber, false));
                    }

                }
            }

            await db.SaveChangesAsync();

            return Ok(new SuccessResponse(true, "File imported successfully."));
        }
        catch (Exception ex)
        {
            return Ok(new SuccessResponse(false, $"Error importing file: {ex.Message}"));
        }
    }
}
