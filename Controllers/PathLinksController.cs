using FWA_Stations.Entities;
using FWA_Stations.Models;
using FWA_Stations.ViewModels.PathLinks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FWA_Stations.Models.Enums;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class PathLinksController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [Authorize(Roles = "VIEW_SUB_DEVICES")]
    [HttpGet("list")]
    public async Task<IActionResult> List(int sub_device_id)
    {
        var paths = await PathLinksQuery()
            .Where(x =>
                x.from_sub_device_id == sub_device_id ||
                x.to_sub_device_id == sub_device_id ||
                x.odfs.Any(o => o.odf_sub_device_id == sub_device_id))
            .OrderByDescending(x => x.id)
            .ToListAsync();

        return Ok(new SuccessResponse(
            true,
            "Data Returned Successfully.",
            paths.Select(MapPathLink).ToList()));
    }

    [Authorize(Roles = "VIEW_SUB_DEVICES")]
    [HttpGet("get/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var path = await PathLinksQuery().FirstOrDefaultAsync(x => x.id == id);

        if (path is null)
            return Ok(new SuccessResponse(false, "Path Not Exist!"));

        return Ok(new SuccessResponse(true, "Data Returned Successfully.", MapPathLink(path)));
    }

    [Authorize(Roles = "ADD_SUB_DEVICE")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] PathLinksVM model)
    {
        var error = await ValidateAsync(model);

        if (error is not null)
            return Ok(new SuccessResponse(false, error));

        var path = new PathLinks
        {
            from_sub_device_id = model.from_sub_device_id,
            to_sub_device_id = model.to_sub_device_id,
            odfs = BuildOdfs(model),
            insert_user_id = GetMyID(),
            insert_date = DateTime.UtcNow
        };

        await db.PathLinks.AddAsync(path);
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Path Added Successfully.", new { path.id }));
    }

    [Authorize(Roles = "EDIT_SUB_DEVICE")]
    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] PathLinksVM model)
    {
        var path = await db.PathLinks
            .Include(x => x.odfs)
            .FirstOrDefaultAsync(x => x.id == id);

        if (path is null)
            return Ok(new SuccessResponse(false, "Path Not Exist!"));

        var error = await ValidateAsync(model);

        if (error is not null)
            return Ok(new SuccessResponse(false, error));

        db.PathLinkOdfs.RemoveRange(path.odfs);

        var newOdfs = BuildOdfs(model);
        newOdfs.ForEach(o => o.path_link_id = path.id);
        await db.PathLinkOdfs.AddRangeAsync(newOdfs);

        path.from_sub_device_id = model.from_sub_device_id;
        path.to_sub_device_id = model.to_sub_device_id;
        path.update_user_id = GetMyID();
        path.update_date = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Path Updated Successfully."));
    }

    [Authorize(Roles = "DELETE_SUB_DEVICE")]
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var path = await db.PathLinks.FirstOrDefaultAsync(x => x.id == id);

        if (path is null)
            return Ok(new SuccessResponse(false, "Path Not Exist!"));

        path.is_delete = true;
        path.delete_user_id = GetMyID();
        path.delete_date = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Path Deleted Successfully."));
    }

    [Authorize(Roles = "VIEW_SUB_DEVICES")]
    [HttpGet("lookup/devices")]
    public async Task<IActionResult> LookupDevices(int type)
    {
        var devices = await db.Devices.AsNoTracking()
            .Where(x => (int)x.type == type)
            .OrderBy(x => x.name)
            .Select(x => new { x.id, x.name })
            .ToListAsync();

        return Ok(new SuccessResponse(true, "Data Returned Successfully.", devices));
    }

    [Authorize(Roles = "VIEW_SUB_DEVICES")]
    [HttpGet("lookup/slots")]
    public async Task<IActionResult> LookupSlots(int device_id)
    {
        var slots = await db.SubDevices.AsNoTracking()
            .Where(x => x.device_id == device_id)
            .Select(x => x.slot ?? "")
            .Distinct()
            .ToListAsync();

        return Ok(new SuccessResponse(
            true,
            "Data Returned Successfully.",
            slots.OrderBy(x => x, NaturalComparer.Instance).ToList()));
    }

    [Authorize(Roles = "VIEW_SUB_DEVICES")]
    [HttpGet("lookup/ports")]
    public async Task<IActionResult> LookupPorts(int device_id, string slot)
    {
        slot ??= "";

        var ports = await db.SubDevices.AsNoTracking()
            .Where(x => x.device_id == device_id && (x.slot ?? "") == slot)
            .Select(x => new
            {
                x.id,
                port = x.port ?? "",
                x.name
            })
            .ToListAsync();

        return Ok(new SuccessResponse(
            true,
            "Data Returned Successfully.",
            ports.OrderBy(x => x.port, NaturalComparer.Instance).ToList()));
    }

    private IQueryable<PathLinks> PathLinksQuery()
    {
        return db.PathLinks.AsNoTracking()
            .Include(x => x.from_sub_device).ThenInclude(x => x.device)
            .Include(x => x.to_sub_device).ThenInclude(x => x.device)
            .Include(x => x.odfs).ThenInclude(x => x.odf_sub_device).ThenInclude(x => x.device)
            .Include(x => x.insert_user)
            .Include(x => x.update_user)
            .AsSplitQuery();
    }

    private static object MapPathLink(PathLinks path)
    {
        return new
        {
            path.id,
            from = MapEndpoint(path.from_sub_device),
            to = MapEndpoint(path.to_sub_device),
            odfs = path.odfs
                .OrderBy(o => o.order)
                .Select(o => new
                {
                    o.order,
                    endpoint = MapEndpoint(o.odf_sub_device)
                })
                .ToList(),
            path.insert_date,
            path.update_date,
            insert_user = GetUserDetails(path.insert_user),
            update_user = GetUserDetails(path.update_user)
        };
    }

    private static object MapEndpoint(SubDevices subDevice)
    {
        if (subDevice is null) return null;

        return new
        {
            sub_device_id = subDevice.id,
            sub_device_name = subDevice.name,
            subDevice.slot,
            subDevice.port,
            subDevice.device_id,
            device_name = subDevice.device?.name,
            device_type = subDevice.device?.type
        };
    }

    private static List<PathLinkOdfs> BuildOdfs(PathLinksVM model)
    {
        return (model.odf_sub_device_ids ?? [])
            .Select((odfId, index) => new PathLinkOdfs
            {
                order = index + 1,
                odf_sub_device_id = odfId
            })
            .ToList();
    }

    private async Task<string> ValidateAsync(PathLinksVM model)
    {
        if (model.from_sub_device_id <= 0)
            return "Please select the 'Connect From' port.";

        if (model.to_sub_device_id <= 0)
            return "Please select the 'Connect To' port.";

        if (model.from_sub_device_id == model.to_sub_device_id)
            return "'Connect From' and 'Connect To' can not be the same port.";

        var odfIds = model.odf_sub_device_ids ?? [];

        if (odfIds.Any(x => x <= 0))
            return "Please complete all ODF rows (ODF, slot and port) or remove the empty ones.";

        if (odfIds.Distinct().Count() != odfIds.Count)
            return "The same ODF port is used more than once in this path.";

        var endpointIds = new[] { model.from_sub_device_id, model.to_sub_device_id };

        var endpointsCount = await db.SubDevices.CountAsync(x => endpointIds.Contains(x.id));

        if (endpointsCount != endpointIds.Length)
            return "Selected device port not found!";

        if (odfIds.Count > 0)
        {
            var validOdfsCount = await db.SubDevices
                .CountAsync(x => odfIds.Contains(x.id) && x.device.type == DeviceType.TransmissionODF);

            if (validOdfsCount != odfIds.Count)
                return "Every ODF row must be a Transmission ODF port.";
        }

        return null;
    }

    private sealed class NaturalComparer : IComparer<string>
    {
        public static readonly NaturalComparer Instance = new();

        public int Compare(string x, string y)
        {
            x ??= "";
            y ??= "";

            int i = 0, j = 0;

            while (i < x.Length && j < y.Length)
            {
                if (char.IsDigit(x[i]) && char.IsDigit(y[j]))
                {
                    int si = i, sj = j;
                    while (i < x.Length && char.IsDigit(x[i])) i++;
                    while (j < y.Length && char.IsDigit(y[j])) j++;

                    var nx = x[si..i].TrimStart('0');
                    var ny = y[sj..j].TrimStart('0');

                    int cmp = nx.Length != ny.Length
                        ? nx.Length.CompareTo(ny.Length)
                        : string.CompareOrdinal(nx, ny);

                    if (cmp != 0) return cmp;
                }
                else
                {
                    int cmp = char.ToUpperInvariant(x[i]).CompareTo(char.ToUpperInvariant(y[j]));
                    if (cmp != 0) return cmp;
                    i++;
                    j++;
                }
            }

            return (x.Length - i).CompareTo(y.Length - j);
        }
    }
}
