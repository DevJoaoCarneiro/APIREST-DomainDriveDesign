using Domain.Entities;
using Domain.Entities.Embeded;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get;  set; } = string.Empty;
        public string Mail { get;  set; } = string.Empty;

        public Address? UserAddress { get;  set; }
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpires { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
