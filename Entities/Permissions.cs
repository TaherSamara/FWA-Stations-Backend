namespace FWA_Stations.Entities;

public class Permissions
{
    public int id { get; set; }
    public string name { get; set; }
    public string code { get; set; }
    public int order { get; set; }
    public string category { get; set; }

    public ICollection<UserPermissions> user_permissions { get; set; }
}
