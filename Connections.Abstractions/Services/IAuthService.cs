using Connections.Abstractions.Contracts.Requests;

namespace Connections.Abstractions.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, string ipAddress, CancellationToken ct = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken ct = default);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken ct = default);
        Task RevokeTokenAsync(RevokeTokenRequest request, string ipAddress, CancellationToken ct = default);
        Task DeleteUserAsync(string userId, CancellationToken ct = default);
        Task<AuthResponse> VerifyOtpAsync(string email, string otp, string ipAddress, CancellationToken ct = default);
        Task DeleteRoleAsync(string roleName, CancellationToken ct = default);
    }
}
