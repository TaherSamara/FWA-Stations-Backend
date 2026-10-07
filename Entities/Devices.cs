using static FWA_Stations.Models.Enums;

namespace FWA_Stations.Entities;

public class Devices : BaseEntity
{
    public int id { get; set; }
    public string name { get; set; }
    public DeviceType type { get; set; }

    public ICollection<SubDevices> sub_devices { get; set; }
}
