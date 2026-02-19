using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.EntityFrameworkCore;
using FWA_Stations.ViewModels.Auth;
using FWA_Stations.Models;
using FWA_Stations.Managers;
using Microsoft.AspNetCore.Authorization;

namespace FWA_Stations.Controllers;

[Route("api/[controller]")]
public class AuthController(IServiceProvider serviceProvider, AuthenticationManager authentication) : BaseController(serviceProvider)
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginVM model)
    {
        var user = await db.Users.AsNoTracking()
            .Include(x => x.user_permissions)
                .ThenInclude(up => up.permission)
            .Where(x => x.email == model.email && x.password == Encrypt(model.password))
            .FirstOrDefaultAsync();

        if (user is null)
            return Ok(new SuccessResponse(false, "These credentials do not match our records!"));

        var image = GetUserImageUrl(user.image);
        var permissions = user.user_permissions?.Select(up => up.permission.code).ToList() ?? [];
        var auth_token = authentication.GenerateAccessToken(user.id, $"{user.first_name} {user.last_name}", user.email, image, permissions);

        var userData = new
        {
            user.id,
            user.first_name,
            user.last_name,
            user.email,
            image,
        };

        return Ok(new SuccessResponse(true, "Login successfully.", new { user = userData, token = auth_token, permissions }));
    }


    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVM model)
    {
        if (model.new_password != model.confirm_password)
            return Ok(new SuccessResponse(false, "New password and confirmation password do not match."));

        if (model.old_password == model.new_password)
            return Ok(new SuccessResponse(false, "New password cannot be the same as old password."));

        var userId = GetMyID();
        var encryptedOldPassword = Encrypt(model.old_password);

        var user = await db.Users.FirstOrDefaultAsync(x => x.id == userId && x.password == encryptedOldPassword);

        if (user is null)
            return Ok(new SuccessResponse(false, "Current password is incorrect."));

        user.password = Encrypt(model.new_password);
        user.update_user_id = userId;
        user.update_date = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(new SuccessResponse(true, "Password changed successfully."));
    }
}
