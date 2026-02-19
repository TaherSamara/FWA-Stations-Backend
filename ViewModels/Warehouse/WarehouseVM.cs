using System.ComponentModel.DataAnnotations;

namespace FWA_Stations.ViewModels.Warehouse;

public class WarehouseVM
{
    [Required]
    public string device_type { get; set; }

    [Required]
    public string serial_number { get; set; }

    public string source_location { get; set; }

    public string customer_line_code { get; set; }

    public string notes { get; set; }

    public int? assigned_user_id { get; set; }
}

public class UpdateCustomerLineCodeVM
{
    [Required]
    public string customer_line_code { get; set; }

    public string notes { get; set; }
}
