namespace FWA_Stations.Entities;

public class Warehouse : BaseEntity
{
    public int id { get; set; }
    public string device_type { get; set; }
    public string serial_number { get; set; }
    public string source_location { get; set; }
    public string customer_line_code { get; set; }
    public string notes { get; set; }

    public int? assigned_user_id { get; set; }
    public Users assigned_user { get; set; }
}
