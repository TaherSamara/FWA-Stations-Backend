using System.Text.Json.Serialization;

namespace FWA_Stations.Models;

public class ErrorResponse(string error, string errorText)
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = error;

    [JsonPropertyName("error_text")]
    public string ErrorText { get; set; } = errorText;
}
