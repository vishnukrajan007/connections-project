using Connections.Abstractions.Contracts.Requests;
using Connections.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;


namespace Connections.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return await _authService.RegisterAsync(request, ip, ct);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return await _authService.LoginAsync(request, ip, ct);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return await _authService.RefreshTokenAsync(request, ip, ct);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(RevokeTokenRequest request, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            await _authService.RevokeTokenAsync(request, ip, ct);
            return Ok();
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId, CancellationToken ct)
        {
            await _authService.DeleteUserAsync(userId, ct);
            return NoContent(); // 204 if success
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<AuthResponse>> VerifyOtp(string email, string otp, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return await _authService.VerifyOtpAsync(email, otp, ip, ct);
        }

        [HttpDelete("role/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName, CancellationToken ct)
        {
            await _authService.DeleteRoleAsync(roleName, ct);
            return NoContent(); // 204
        }
    }
}
