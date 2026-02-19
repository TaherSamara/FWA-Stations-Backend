using System.ComponentModel.DataAnnotations;

namespace FWA_Stations.ViewModels.Users;

public class UsersVM
{
    [Required]
    public string first_name { get; set; }
    
    [Required]
    public string last_name { get; set; }
    
    [Required]
    [EmailAddress]
    public string email { get; set; }
    
    public IFormFile image { get; set; }

    public List<int> permission_ids { get; set; } = new();
}