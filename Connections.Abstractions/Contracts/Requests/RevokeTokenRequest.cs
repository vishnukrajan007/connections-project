using System.ComponentModel.DataAnnotations;

namespace Connections.Abstractions.Contracts.Requests
{
    public class RevokeTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = default!;
    }
}
