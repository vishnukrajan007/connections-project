using System.ComponentModel.DataAnnotations;

namespace Connections.Abstractions.Contracts.Requests
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
