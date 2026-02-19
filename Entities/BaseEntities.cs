namespace FWA_Stations.Entities;

public abstract class BaseEntity
{
    public bool is_delete { get; set; } = false;

    public DateTime? insert_date { get; set; }
    public DateTime? update_date { get; set; }
    public DateTime? delete_date { get; set; }

    public int? insert_user_id { get; set; }
    public Users insert_user { get; set; }

    public int? update_user_id { get; set; }
    public Users update_user { get; set; }

    public int? delete_user_id { get; set; }
    public Users delete_user { get; set; }
}
