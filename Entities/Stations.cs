namespace FWA_Stations.Entities;

public class Stations : BaseEntity
{
    public int id { get; set; }
    public string name { get; set; }

    public ICollection<Subscribers> subscribers { get; set; }
}
