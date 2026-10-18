namespace FWA_Stations.ViewModels.PathLinks;

public class PathLinksVM
{
    public int from_sub_device_id { get; set; }
    public int to_sub_device_id { get; set; }
    public List<int> odf_sub_device_ids { get; set; } = [];
}
