namespace FWA_Stations.Entities;

public class UserPermissions
{
    public int id { get; set; }
    public int user_id { get; set; }
    public int permission_id { get; set; }

    public Users user { get; set; }
    public Permissions permission { get; set; }
}
