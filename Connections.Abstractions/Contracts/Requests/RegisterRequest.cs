using Connections.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Connections.Abstractions.Contracts.Requests
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required, MinLength(6)]
        public string Password { get; set; } = default!;

        public string? FullName { get; set; }

        public string Role { get; set; } = Roles.User;
    }
}
