using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.Response
{
    public class UserResponseDTO
    {
        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("Status")]
        public string Status { get; set; } = string.Empty;

        public UserData? Data { get; set; } = new UserData();
    }

    public class UserData
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("Mail")]
        public string Mail { get; set; } = string.Empty;
    }
}
