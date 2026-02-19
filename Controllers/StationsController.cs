using FWA_Stations.Entities;
using FWA_Stations.Models;
using FWA_Stations.ViewModels.Stations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class StationsController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [Authorize(Roles = "VIEW_STATIONS")]
    [HttpGet("list")]
    public async Task<IActionResult> List(string q, int size = 20, int page = 1)
    {
        IQueryable<Stations> query = db.Stations.AsNoTracking()
            .Include(x => x.insert_user)
            .Include(x => x.update_user);

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
                x.insert_date,
                x.update_date,
                insert_user = GetUserDetails(x.insert_user),
                update_user = GetUserDetails(x.update_user)
            }).ToList();

        vm.total_records = await query.CountAsync();

        return Ok(new SuccessResponse(true, "Data Returned", vm));
    }


    [Authorize(Roles = "ADD_STATION")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] StationsVM model)
    {
        var station = await db.Stations.AnyAsync(x => x.name == model.name);

        if (station)
            return Ok(new SuccessResponse(false, "Station Already Exists"));

        var newStation = new Stations
        {
            name = model.name,
            insert_user_id = GetMyID(),
            insert_date = DateTime.UtcNow
        };

        await db.Stations.AddAsync(newStation);
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Station Added Successfully"));
    }


    [Authorize(Roles = "EDIT_STATION")]
    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] StationsVM model)
    {
        var station = db.Stations.FirstOrDefault(x => x.id == id);

        if (station is null)
            return Ok(new SuccessResponse(false, "Station Not Exist"));

        station.name = model.name;
        station.update_user_id = GetMyID();
        station.update_date = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Station Updated Successfully"));
    }


    [Authorize(Roles = "DELETE_STATION")]
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var station = db.Stations.FirstOrDefault(x => x.id == id);

        if (station is null)
            return Ok(new SuccessResponse(false, "Station Not Exist"));

        station.is_delete = true;
        station.delete_user_id = GetMyID();
        station.delete_date = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Station Deleted Successfully"));
    }
}
