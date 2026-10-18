namespace FWA_Stations.Entities;

public class PathLinkOdfs
{
    public int id { get; set; }
    public int order { get; set; }

    public int path_link_id { get; set; }
    public PathLinks path_link { get; set; }

    public int odf_sub_device_id { get; set; }
    public SubDevices odf_sub_device { get; set; }
}
