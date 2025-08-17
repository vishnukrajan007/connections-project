using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Connections.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? OtpCode { get; set; }      // store OTP
        public DateTime? OtpExpiry { get; set; }  // store expiry time
    }
}
