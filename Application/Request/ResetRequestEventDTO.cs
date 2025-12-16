using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Request
{
    public class ResetRequestEventDTO
    {
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
