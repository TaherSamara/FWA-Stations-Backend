using FWA_Stations.Entities;
using FWA_Stations.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class CommonController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [HttpGet("permissions")]
    public async Task<IActionResult> List()
    {
        var permissions = await db.Permissions.AsNoTracking()
             .GroupBy(x => x.category)
             .Select(g => new
             {
                 category = g.Key,
                 minOrder = g.Min(x => x.order),
                 permissions = g
                     .OrderBy(x => x.order)
                     .Select(p => new
                     {
                         p.id,
                         p.name,
                         p.code
                     }).ToList()
             })
             .OrderBy(x => x.minOrder)
             .Select(x => new
             {
                 x.category,
                 x.permissions
             }).ToListAsync();

        return Ok(new SuccessResponse(true, "Data Returned", permissions));
    }


    [HttpGet("search")]
    public async Task<IActionResult> GeneralSearch(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(new SuccessResponse(false, "Search query is required"));

        string pattern = $"%{q.Trim()}%";
        var currentUserId = GetMyID();

        // Search in Users (only if has permission)
        var users = new List<dynamic>();
        if (CanView("VIEW_USERS"))
        {
            users = await db.Users.AsNoTracking()
                .Where(x => EF.Functions.Like(x.first_name, pattern) 
                    || EF.Functions.Like(x.last_name, pattern)
                    || EF.Functions.Like(x.email, pattern))
                .Take(10)
                .Select(x => new
                {
                    x.id,
                    type = "User",
                    title = $"{x.first_name} {x.last_name}",
                    subtitle = x.email,
                    image = GetUserImageUrl(x.image)
                })
                .ToListAsync<dynamic>();
        }

        // Search in Stations (only if has permission)
        var stations = new List<dynamic>();
        if (CanView("VIEW_STATIONS"))
        {
            stations = await db.Stations.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, pattern))
                .Take(10)
                .Select(x => new
                {
                    x.id,
                    type = "Station",
                    title = x.name,
                    subtitle = $"Created: {x.insert_date:yyyy-MM-dd}",
                    image = (string)null
                })
                .ToListAsync<dynamic>();
        }

        // Search in Subscribers (only if has permission)
        var subscribers = new List<dynamic>();
        if (CanView("VIEW_SUBSCRIBERS"))
        {
            subscribers = await db.Subscribers.AsNoTracking()
                .Include(x => x.station)
                .Where(x => EF.Functions.Like(x.name, pattern)
                    || EF.Functions.Like(x.line_code, pattern)
                    || EF.Functions.Like(x.management_ip, pattern))
                .Take(10)
                .Select(x => new
                {
                    x.id,
                    type = "Subscriber",
                    title = x.name,
                    subtitle = $"Line Code: {x.line_code} | Station: {x.station.name}",
                    image = (string)null
                })
                .ToListAsync<dynamic>();
        }

        // Search in Warehouse/Devices (with permission-based filtering)
        var devices = new List<dynamic>();
        if (CanView("VIEW_ALL_DEVICES") || CanView("VIEW_MY_DEVICES"))
        {
            IQueryable<Warehouse> deviceQuery = db.Warehouse.AsNoTracking()
                .Include(x => x.assigned_user)
                .Where(x => EF.Functions.Like(x.device_type, pattern)
                    || EF.Functions.Like(x.serial_number, pattern)
                    || EF.Functions.Like(x.customer_line_code, pattern));

            // If user only has VIEW_MY_DEVICES, show only their devices
            if (!CanView("VIEW_ALL_DEVICES"))
            {
                deviceQuery = deviceQuery.Where(x => x.assigned_user_id == currentUserId);
            }

            devices = await deviceQuery
                .Take(10)
                .Select(x => new
                {
                    x.id,
                    type = "Device",
                    title = x.device_type,
                    subtitle = $"Serial: {x.serial_number} | Customer: {x.customer_line_code}",
                    image = (string)null
                })
                .ToListAsync<dynamic>();
        }

        // Combine all results
        var results = new
        {
            users = new
            {
                count = users.Count,
                items = users
            },
            stations = new
            {
                count = stations.Count,
                items = stations
            },
            subscribers = new
            {
                count = subscribers.Count,
                items = subscribers
            },
            devices = new
            {
                count = devices.Count,
                items = devices
            },
            total = users.Count + stations.Count + subscribers.Count + devices.Count
        };

        return Ok(new SuccessResponse(true, "Search Results", results));
    }
}
