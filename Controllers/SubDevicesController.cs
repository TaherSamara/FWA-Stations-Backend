using FWA_Stations.Entities;
using FWA_Stations.Models;
using FWA_Stations.ViewModels.SubDevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class SubDevicesController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [Authorize(Roles = "VIEW_SUB_DEVICES")]
    [HttpGet("list")]
    public async Task<IActionResult> List(string q, int size = 20, int page = 1, int? device_id = null)
    {
        IQueryable<SubDevices> query = db.SubDevices
            .AsNoTracking()
            .Include(x => x.device)
            .Include(x => x.insert_user)
            .Include(x => x.update_user);

        vm.total_count = await query.CountAsync();

        if (!string.IsNullOrEmpty(q))
        {
            string pattern = $"%{q.Trim()}%";

            query = query.Where(x =>
                EF.Functions.Like(x.name, pattern) ||
                EF.Functions.Like(x.station_name, pattern) ||
                EF.Functions.Like(x.line_code, pattern) ||
                EF.Functions.Like(x.order_number, pattern) ||
                EF.Functions.Like(x.contact_person, pattern) ||
                EF.Functions.Like(x.contact_person_phone_no, pattern));
        }

        if (device_id.HasValue)
        {
            query = query.Where(x => x.device_id == device_id.Value);
        }

        vm.total_records = await query.CountAsync();

        vm.data = query
            .OrderByDescending(x => x.id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsEnumerable()
            .Select(x => new
            {
                x.id,

                x.name,
                x.station_name,
                x.line_code,
                x.order_number,

                x.unit_type,
                x.unit_direction,
                x.link_mac_address,

                x.connection_type,
                x.speed,

                x.management_ip,
                x.management_vlan,
                x.wan_ip,

                x.mikrotik_id,
                x.mikrotik_mac_address,

                x.sas_port,

                x.odf_name,
                x.odf_port,

                x.customer_address,
                x.contact_person,
                x.contact_person_phone_no,

                x.other_details,

                x.insert_date,
                x.update_date,

                device = GetDeviceDetails(x.device),
                insert_user = GetUserDetails(x.insert_user),
                update_user = GetUserDetails(x.update_user)
            })
            .ToList();

        return Ok(new SuccessResponse(
            true,
            "Data Returned Successfully.",
            vm));
    }


    [Authorize(Roles = "ADD_SUB_DEVICE")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] SubDevicesVM model)
    {
        var subDeviceExists = await db.SubDevices.AnyAsync(x =>
            x.name == model.name &&
            x.device_id == model.device_id);

        if (subDeviceExists)
        {
            return Ok(new SuccessResponse(
                false,
                "Sub Device Already Exists!"));
        }

        var deviceExists = await db.Devices.AnyAsync(x =>
            x.id == model.device_id);

        if (!deviceExists)
        {
            return Ok(new SuccessResponse(
                false,
                "Device Not Found!"));
        }

        var newSubDevice = new SubDevices
        {
            name = model.name,
            station_name = model.station_name,
            line_code = model.line_code,
            order_number = model.order_number,

            unit_type = model.unit_type,
            unit_direction = model.unit_direction,
            link_mac_address = model.link_mac_address,

            connection_type = model.connection_type,
            speed = model.speed,

            management_ip = model.management_ip,
            management_vlan = model.management_vlan,
            wan_ip = model.wan_ip,

            mikrotik_id = model.mikrotik_id,
            mikrotik_mac_address = model.mikrotik_mac_address,

            sas_port = model.sas_port,

            odf_name = model.odf_name,
            odf_port = model.odf_port,

            customer_address = model.customer_address,
            contact_person = model.contact_person,
            contact_person_phone_no = model.contact_person_phone_no,

            other_details = model.other_details,

            device_id = model.device_id,
            insert_user_id = GetMyID(),
            insert_date = DateTime.UtcNow
        };

        await db.SubDevices.AddAsync(newSubDevice);
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(
            true,
            "Sub Device Added Successfully."));
    }


    [Authorize(Roles = "EDIT_SUB_DEVICE")]
    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(
        int id,
        [FromBody] SubDevicesVM model)
    {
        var subDevice = await db.SubDevices
            .FirstOrDefaultAsync(x => x.id == id);

        if (subDevice is null)
        {
            return Ok(new SuccessResponse(
                false,
                "Sub Device Not Exist!"));
        }

        var deviceExists = await db.Devices.AnyAsync(x =>
            x.id == model.device_id);

        if (!deviceExists)
        {
            return Ok(new SuccessResponse(
                false,
                "Device Not Found!"));
        }

        subDevice.name = model.name;
        subDevice.station_name = model.station_name;
        subDevice.line_code = model.line_code;
        subDevice.order_number = model.order_number;

        subDevice.unit_type = model.unit_type;
        subDevice.unit_direction = model.unit_direction;
        subDevice.link_mac_address = model.link_mac_address;

        subDevice.connection_type = model.connection_type;
        subDevice.speed = model.speed;

        subDevice.management_ip = model.management_ip;
        subDevice.management_vlan = model.management_vlan;
        subDevice.wan_ip = model.wan_ip;

        subDevice.mikrotik_id = model.mikrotik_id;
        subDevice.mikrotik_mac_address =
            model.mikrotik_mac_address;

        subDevice.sas_port = model.sas_port;

        subDevice.odf_name = model.odf_name;
        subDevice.odf_port = model.odf_port;

        subDevice.customer_address = model.customer_address;
        subDevice.contact_person = model.contact_person;
        subDevice.contact_person_phone_no =
            model.contact_person_phone_no;

        subDevice.other_details = model.other_details;

        subDevice.device_id = model.device_id;
        subDevice.update_user_id = GetMyID();
        subDevice.update_date = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(
            true,
            "Sub Device Updated Successfully."));
    }


    [Authorize(Roles = "DELETE_SUB_DEVICE")]
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var subDevice = await db.SubDevices
            .FirstOrDefaultAsync(x => x.id == id);

        if (subDevice is null)
        {
            return Ok(new SuccessResponse(
                false,
                "Sub Device Not Exist!"));
        }

        subDevice.is_delete = true;
        subDevice.delete_user_id = GetMyID();
        subDevice.delete_date = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(
            true,
            "Sub Device Deleted Successfully."));
    }


    [HttpPost("ping/{ip}")]
    public async Task<IActionResult> Ping(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            return Ok(new SuccessResponse(
                false,
                "IP is required!"));
        }

        if (!System.Net.IPAddress.TryParse(ip, out _))
        {
            return Ok(new SuccessResponse(
                false,
                "Invalid IP address format!"));
        }

        var stopwatch = Stopwatch.StartNew();

        var startInfo = new ProcessStartInfo
        {
            FileName = "ping",
            Arguments = $"-n 4 -w 1000 {ip}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await Task.WhenAll(outputTask, errorTask);
        await process.WaitForExitAsync();

        stopwatch.Stop();

        var pingOutput = outputTask.Result;
        var errorOutput = errorTask.Result;
        var exitCode = process.ExitCode;

        bool pingable =
            exitCode == 0 &&
            !pingOutput.Contains(
                "Request timed out",
                StringComparison.OrdinalIgnoreCase);

        string min = null;
        string avg = null;
        string max = null;

        var timeMatch = Regex.Match(
            pingOutput,
            @"Minimum = (\d+)ms, Maximum = (\d+)ms, Average = (\d+)ms",
            RegexOptions.IgnoreCase);

        if (timeMatch.Success)
        {
            min = timeMatch.Groups[1].Value;
            max = timeMatch.Groups[2].Value;
            avg = timeMatch.Groups[3].Value;
        }

        int? sent = null;
        int? received = null;
        int? lost = null;
        int? lossPercent = null;

        var packetMatch = Regex.Match(
            pingOutput,
            @"Sent = (\d+), Received = (\d+), Lost = (\d+) \((\d+)% loss\)",
            RegexOptions.IgnoreCase);

        if (packetMatch.Success)
        {
            sent = int.Parse(packetMatch.Groups[1].Value);
            received = int.Parse(packetMatch.Groups[2].Value);
            lost = int.Parse(packetMatch.Groups[3].Value);
            lossPercent = int.Parse(packetMatch.Groups[4].Value);
        }

        string ttl = null;

        var ttlMatch = Regex.Match(
            pingOutput,
            @"TTL=(\d+)",
            RegexOptions.IgnoreCase);

        if (ttlMatch.Success)
        {
            ttl = ttlMatch.Groups[1].Value;
        }

        var data = new
        {
            ip,
            execution_time_ms = stopwatch.ElapsedMilliseconds,
            exit_code = exitCode,
            ping = new
            {
                status = pingable
                    ? "Pingable"
                    : "Not Reachable",

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
                error = string.IsNullOrWhiteSpace(errorOutput)
                    ? null
                    : errorOutput
            }
        };

        return Ok(new SuccessResponse(
            true,
            "Ping data returned",
            data));
    }
}
