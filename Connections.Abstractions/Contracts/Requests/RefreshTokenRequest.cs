using System.ComponentModel.DataAnnotations;

namespace Connections.Abstractions.Contracts.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = default!;
    }
}
