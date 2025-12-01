using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.Response
{
    public class UserListResponseDTO
    {
        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("Status")]
        public string Status { get; set; } = string.Empty;

        public List<UserDataList?> Data { get; set; } = new List<UserDataList>();
    }

    public class UserDataList
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("Mail")]
        public string Mail { get; set; } = string.Empty;
    }
}
