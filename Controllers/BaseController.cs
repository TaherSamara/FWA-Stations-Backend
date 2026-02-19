using FWA_Stations.Data;
using FWA_Stations.Entities;
using FWA_Stations.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FWA_Stations.Controllers;

[ApiController]
[Authorize]
public class BaseController(IServiceProvider serviceProvider) : ControllerBase, IDisposable
{
    /********************************************************************
    *                   Developed By Taher B. Samara                    *
    ********************************************************************/
    protected ListVM vm = new();
    protected readonly Random random = new();
    protected readonly DataContext db = serviceProvider.GetRequiredService<DataContext>();
    protected readonly HttpClient httpClient = serviceProvider.GetRequiredService<HttpClient>();
    protected readonly IWebHostEnvironment environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

    protected static readonly string frontendBaseUrl = "http://tahersamara.com";
    protected static readonly string backendBaseUrl = "http://craftmyapi.com";

    protected int GetMyID()
    {
        var claim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
        return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
    }

    protected string GetMyName()
    {
        var name = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        return string.IsNullOrWhiteSpace(name) ? "Unknown" : name;
    }

    protected string GetMyImage()
    {
        var image = User.Claims.FirstOrDefault(x => x.Type == "image")?.Value;
        return image ?? string.Empty;
    }

    protected bool CanView(string permissionCode)
    {
        var userPermissions = User.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();

        return userPermissions.Contains(permissionCode);
    }

    protected string Encrypt(string plainText)
    {
        string password = "encryptionkey";
        byte[] salt = Encoding.UTF8.GetBytes("Ivan Medvedev");
        byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

        using var aes = Aes.Create();

        aes.Key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            20000,
            HashAlgorithmName.SHA256,
            32
        );

        aes.IV = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            20000,
            HashAlgorithmName.SHA256,
            16
        );

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(plainBytes, 0, plainBytes.Length);
        cs.FlushFinalBlock();

        return Convert.ToBase64String(ms.ToArray());
    }

    protected string Decrypt(string cipherText)
    {
        string password = "encryptionkey";
        byte[] salt = Encoding.UTF8.GetBytes("Ivan Medvedev");
        byte[] cipherBytes = Convert.FromBase64String(cipherText.Replace(" ", "+"));

        using var aes = Aes.Create();

        aes.Key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            20000,
            HashAlgorithmName.SHA256,
            32
        );

        aes.IV = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            20000,
            HashAlgorithmName.SHA256,
            16
        );

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(cipherBytes, 0, cipherBytes.Length);
        cs.FlushFinalBlock();

        return Encoding.Unicode.GetString(ms.ToArray());
    }

    protected static string GetUserImageUrl(string fileName)
    {
        return string.IsNullOrEmpty(fileName)
            ? $"{backendBaseUrl}/content/images/no-user.png"
            : $"{backendBaseUrl}/content/images/users/{fileName}";
    }

    protected async Task<string> UploadImageAsync(IFormFile image, string folderName, string oldImage = "")
    {
        if (image is null) return oldImage;

        string folderPath = Path.Combine(environment.WebRootPath, "Content", "Images", folderName);
        Directory.CreateDirectory(folderPath);

        string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
        string filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        if (!string.IsNullOrWhiteSpace(oldImage))
        {
            string oldPath = Path.Combine(folderPath, oldImage);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }

        return fileName;
    }

    protected string RandomPassord(int length)
    {
        const string chars = "abc0123456789";
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
    }

    protected static object GetUserDetails(Users user)
    {
        if (user is null) return null;

        return new
        {
            user.id,
            user.first_name,
            user.last_name,
            user.full_name,
            user.email,
            image = GetUserImageUrl(user.image)
        };
    }

    protected static object GetStationDetails(Stations station)
    {
        if (station is null) return null;

        return new
        {
            station.id,
            station.name
        };
    }

    protected string GenerateUsername(string firstName, string lastName)
    {
        string username = $"{firstName?.ToLower().Trim() ?? ""}.{lastName?.ToLower().Trim() ?? ""}";
        return username.Replace(" ", "");
    }

    public void Dispose()
    {
        db.Dispose();
    }
}
