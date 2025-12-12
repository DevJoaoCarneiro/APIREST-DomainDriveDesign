using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Response
{
    public class AuthResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public GoogleUserData UserData { get; set; } = new GoogleUserData();
    }

    public class GoogleUserData
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }

        public UserPayload Payload { get; set; } = new UserPayload();
    }

    public class UserPayload
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string GoogleId { get; set; } = string.Empty;
    }
}
