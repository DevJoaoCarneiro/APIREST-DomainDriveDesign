using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Request
{
    public class CompletePasswordResetRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
