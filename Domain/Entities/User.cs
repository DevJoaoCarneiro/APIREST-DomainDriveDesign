using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.entities
{
    public class User
    {
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Mail { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;

        public User()
        {
        }

        public User(string Name, string Mail, string PasswordHash)
        {
            this.UserId = Guid.NewGuid();
            this.Name = Name;
            this.Mail = Mail;
            this.PasswordHash = PasswordHash;
        }


    }
}
