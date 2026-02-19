using System.ComponentModel.DataAnnotations.Schema;

namespace FWA_Stations.Entities;

public class Users : BaseEntity
{
    public int id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string email { get; set; }
    public string image { get; set; }
    public string password { get; set; }

    public ICollection<UserPermissions> user_permissions { get; set; }

    [NotMapped]
    public string full_name => first_name + " " + last_name;
}
