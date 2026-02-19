using System.ComponentModel.DataAnnotations;

namespace FWA_Stations.ViewModels.Stations;

public class StationsVM
{
    [Required]
    public string name { get; set; }
}
