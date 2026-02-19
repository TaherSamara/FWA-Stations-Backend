using static FWA_Stations.Models.Enums;

namespace FWA_Stations.Entities;

public class Subscribers : BaseEntity
{
    public int id { get; set; }
    public string name { get; set; }
    public string line_code { get; set; }
    public string unit_type { get; set; }
    public string link_mac_address { get; set; }
    public string unit_direction { get; set; }
    public string management_ip { get; set; }
    public string mikrotik_id { get; set; }
    public string mikrotik_mac_address { get; set; }
    public string sas_name { get; set; }
    public string sas_port { get; set; }
    public string odf_name { get; set; }
    public string odf_port { get; set; }
    public string management_vlan { get; set; }
    public ServiceType service_type { get; set; }

    public string notes { get; set; }

    public int station_id { get; set; }
    public Stations station { get; set; }
}