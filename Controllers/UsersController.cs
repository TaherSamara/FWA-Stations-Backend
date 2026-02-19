using FWA_Stations.Entities;
using FWA_Stations.Models;
using FWA_Stations.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class UsersController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [Authorize(Roles = "VIEW_USERS")]
    [HttpGet("list")]
    public async Task<IActionResult> List(string q, int size = 20, int page = 1)
    {
        IQueryable<Users> query = db.Users.AsNoTracking()
            .Include(x => x.insert_user)
            .Include(x => x.update_user)
            .Include(x => x.user_permissions)
                .ThenInclude(up => up.permission);

        vm.total_count = await query.CountAsync();

        if (!string.IsNullOrEmpty(q))
        {
            string pattern = $"%{q.Trim()}%";
            query = query.Where(x => EF.Functions.Like(x.first_name, pattern)
                || EF.Functions.Like(x.last_name, pattern)
                || EF.Functions.Like(x.email, pattern));
        }

        vm.data = query
            .OrderByDescending(x => x.id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsEnumerable()
            .Select(x => new
            {
                x.id,
                x.first_name,
                x.last_name,
                x.email,
                image = GetUserImageUrl(x.image),
                permissions = x.user_permissions?
                    .Select(up => up.permission)
                    .OrderBy(p => p.order)
                    .Select(p => new
                    {
                        p.id,
                        p.name,
                        p.code,
                        p.category
                    }).ToList() ?? [],
                x.insert_date,
                x.update_date,
                insert_user = GetUserDetails(x.insert_user),
                update_user = GetUserDetails(x.update_user)
            }).ToList();

        vm.total_records = await query.CountAsync();

        return Ok(new SuccessResponse(true, "Data Returned", vm));
    }


    [Authorize(Roles = "ADD_USER")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromForm] UsersVM model)
    {
        var userExist = await db.Users.AnyAsync(x => x.email == model.email);

        if (userExist)
            return Ok(new SuccessResponse(false, "User Already Exists"));

        var newUser = new Users
        {
            first_name = model.first_name?.Trim() ?? "",
            last_name = model.last_name?.Trim() ?? "",
            email = model.email?.ToLower().Trim() ?? "",
            image = await UploadImageAsync(model.image, "Users"),
            password = Encrypt($"{model.first_name.ToLower()}@123"),
            insert_user_id = GetMyID(),
            insert_date = DateTime.UtcNow
        };

        await db.Users.AddAsync(newUser);
        await db.SaveChangesAsync();

        // Add user permissions
        if (model.permission_ids != null && model.permission_ids.Any())
        {
            var userPermissions = model.permission_ids.Select(permId => new UserPermissions
            {
                user_id = newUser.id,
                permission_id = permId
            }).ToList();

            await db.UserPermissions.AddRangeAsync(userPermissions);
            await db.SaveChangesAsync();
        }

        return Ok(new SuccessResponse(true, "User Added Successfully"));
    }


    [Authorize(Roles = "EDIT_USER")]
    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, [FromForm] UsersVM model)
    {
        var user = await db.Users
            .Include(x => x.user_permissions)
            .FirstOrDefaultAsync(x => x.id == id);

        if (user is null)
            return Ok(new SuccessResponse(false, "User Not Exist"));

        var emailExists = await db.Users.AnyAsync(x => x.email == model.email && x.id != id);

        if (emailExists)
            return Ok(new SuccessResponse(false, "Email Already Exists"));

        user.first_name = model.first_name?.Trim() ?? "";
        user.last_name = model.last_name?.Trim() ?? "";
        user.email = model.email?.ToLower().Trim() ?? "";
        user.image = await UploadImageAsync(model.image, "Users", user.image);
        user.update_user_id = GetMyID();
        user.update_date = DateTime.UtcNow;

        // Remove old permissions
        if (user.user_permissions != null && user.user_permissions.Any())
        {
            db.UserPermissions.RemoveRange(user.user_permissions);
        }

        // Add new permissions
        if (model.permission_ids != null && model.permission_ids.Any())
        {
            var userPermissions = model.permission_ids.Select(permId => new UserPermissions
            {
                user_id = user.id,
                permission_id = permId
            }).ToList();

            await db.UserPermissions.AddRangeAsync(userPermissions);
        }

        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "User Updated Successfully"));
    }


    [Authorize(Roles = "DELETE_USER")]
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.id == id);

        if (user is null)
            return Ok(new SuccessResponse(false, "User Not Exist"));

        user.is_delete = true;
        user.delete_user_id = GetMyID();
        user.delete_date = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "User Deleted Successfully"));
    }
}
