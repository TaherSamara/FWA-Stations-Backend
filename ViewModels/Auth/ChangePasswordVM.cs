using System.ComponentModel.DataAnnotations;

namespace FWA_Stations.ViewModels.Auth;

public class ChangePasswordVM
{
    [Required]
    public string old_password { get; set; }
    
    [Required]
    public string new_password { get; set; }
    
    [Required]
    public string confirm_password { get; set; }
}
