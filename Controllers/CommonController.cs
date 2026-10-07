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

        // Search in Devices (only if has permission)
        var devices = new List<dynamic>();
        if (CanView("VIEW_DEVICES"))
        {
            devices = await db.Devices.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, pattern))
                .Take(10)
                .Select(x => new
                {
                    x.id,
                    title = x.name,
                    type = "Device",
                    subtitle = $"Type: {x.type.ToString()}",
                    image = (string)null
                }).ToListAsync<dynamic>();
        }

        // Search in SubDevices (only if has permission)
        var subDevices = new List<dynamic>();
        if (CanView("VIEW_SUB_DEVICES"))
        {
            subDevices = await db.SubDevices.AsNoTracking()
                .Include(x => x.device)
                .Where(x => EF.Functions.Like(x.name, pattern)
                    || EF.Functions.Like(x.line_code, pattern)
                    || EF.Functions.Like(x.management_ip, pattern))
                .Take(10)
                .Select(x => new
                {
                    x.id,
                    type = "SubDevice",
                    title = x.name,
                    subtitle = $"Device: {x.device.name} | Line Code: {x.line_code}",
                    image = (string)null
                }).ToListAsync<dynamic>();
        }

        // Combine all results
        var results = new
        {
            devices = new
            {
                count = devices.Count,
                items = devices
            },
            subDevices = new
            {
                count = subDevices.Count,
                items = subDevices
            },
            total = devices.Count + subDevices.Count
        };

        return Ok(new SuccessResponse(true, "Search Results", results));
    }
}