using System.Text.Json.Serialization;

namespace Application.Response
{
    public class RefreshTokenResponseDTO
    {

        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("Status")]
        public string Status { get; set; } = string.Empty;

        public DataToken Data { get; set; } = new DataToken();
    }

    public class DataToken
    {
        [JsonPropertyName("AccessToken")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonPropertyName("RefreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }

}
