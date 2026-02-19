using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FWA_Stations.Managers;

public class AuthenticationManager(IConfiguration configuration)
{
    private readonly string secretKey = configuration["JwtSettings:SecretKey"];
    private readonly string issuer = configuration["JwtSettings:Issuer"];
    private readonly string audience = configuration["JwtSettings:Audience"];

    public string GenerateAccessToken(int id, string name, string email, string image, List<string> roles = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, id.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new("image", image),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString().ToLower())
        };

        if (roles is not null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken
        (
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddYears(10),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}