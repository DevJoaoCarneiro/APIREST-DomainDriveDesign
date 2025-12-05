using Application.Request;
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
        [JsonPropertyName("UserId")]
        public Guid UserId { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("Mail")]
        public string Mail { get; set; } = string.Empty;

        [JsonPropertyName("Address")]
        public AddressResponseDTO UserAddress { get; set; }
    }

    public class AddressResponseDTO
    {
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }
}
