using System.Text.Json.Serialization;

namespace FWA_Stations.Models;

public class SuccessResponse(bool success, string message, dynamic data = null)
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = success;

    [JsonPropertyName("msg")]
    public string Message { get; set; } = message;

    [JsonPropertyName("data")]
    public dynamic Data { get; set; } = data;
}
