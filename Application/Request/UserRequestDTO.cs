using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Request
{
    public class UserRequestDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Mail { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;
    }
}
