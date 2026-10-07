namespace FWA_Stations.ViewModels.SubDevices;

public class SubDevicesVM
{
    // Basic Information
    public string name { get; set; }
    public string station_name { get; set; }
    public string line_code { get; set; }
    public string order_number { get; set; }

    // Unit Information
    public string unit_type { get; set; }
    public string unit_direction { get; set; }
    public string link_mac_address { get; set; }

    // Connection Information
    public string connection_type { get; set; }
    public string speed { get; set; }

    // Network Information
    public string management_ip { get; set; }
    public string management_vlan { get; set; }
    public string wan_ip { get; set; }

    // MikroTik Information
    public string mikrotik_id { get; set; }
    public string mikrotik_mac_address { get; set; }

    // SAS Information
    public string sas_port { get; set; }

    // ODF Information
    public string odf_name { get; set; }
    public string odf_port { get; set; }

    // Customer Information
    public string customer_address { get; set; }
    public string contact_person { get; set; }
    public string contact_person_phone_no { get; set; }

    // Additional Information
    public string other_details { get; set; }

    public int device_id { get; set; }
}
