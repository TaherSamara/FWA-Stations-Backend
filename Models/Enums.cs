namespace FWA_Stations.Models;

public class Enums
{
    public enum DeviceType
    {
        TransmissionODF = 10,
        TransmissionNode = 20,
        FTTHNode = 30,
        ISAM = 40,
        Router = 50,
        SAS = 60,
        Station = 70,
        PTP = 80,
        Mobadara = 90
    }

    public enum ServiceType
    {
        None = 0,
        Mobadara = 10,
        PTP = 20,
        BaseStation = 30
    }
}
