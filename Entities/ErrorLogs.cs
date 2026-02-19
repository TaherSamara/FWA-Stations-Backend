namespace FWA_Stations.Entities;

public class ErrorLogs : BaseEntity
{
    public int id { get; set; }

    // Short error message (e.g. Exception.Message)
    public string message { get; set; }

    // Full error details or stack trace
    public string details { get; set; }

    // Source of the error (Job/Service/Controller)
    public string source { get; set; }

    // Operation type (WhatsApp, Email, DB, etc.)
    public string operation { get; set; }

    // Related entity type (User, Client, Project, etc.)
    public string entity_type { get; set; }

    // Related entity ID (e.g. userId, projectId)
    public int? entity_id { get; set; }
}
