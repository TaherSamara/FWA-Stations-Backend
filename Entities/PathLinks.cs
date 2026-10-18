namespace FWA_Stations.Entities;

public class PathLinks : BaseEntity
{
    public int id { get; set; }

    public int from_sub_device_id { get; set; }
    public SubDevices from_sub_device { get; set; }

    public int to_sub_device_id { get; set; }
    public SubDevices to_sub_device { get; set; }

    public ICollection<PathLinkOdfs> odfs { get; set; }
}
