using Connections.Abstractions.Contracts.Requests;
using Connections.Abstractions.Services;
using Connections.Domain.Constants;
using Connections.Domain.Entities;
using Connections.Infrastructure.Identity;
using Connections.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Connections.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly JwtTokenFactory _jwtFactory;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            JwtTokenFactory jwtFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _jwtFactory = jwtFactory;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string ipAddress, CancellationToken ct = default)
        {
            var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(";", result.Errors.Select(e => e.Description)));
            }

            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(request.Role));
            }

            await _userManager.AddToRoleAsync(user, request.Role);
            return await GenerateTokensAsync(user, ipAddress, ct);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return await GenerateTokensAsync(user, ipAddress, ct);
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken ct = default)
        {
            var refreshToken = await _db.RefreshTokens.Include(r => r.UserId)
                .FirstOrDefaultAsync(r => r.Token == request.RefreshToken, ct);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            // revoke old token
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            var newToken = _jwtFactory.CreateRefreshToken();
            refreshToken.ReplacedByToken = newToken.token;

            // save new token
            var rt = new RefreshToken
            {
                Token = newToken.token,
                Expires = newToken.expiresAt,
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserId = user.Id
            };

            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync(ct);

            return await GenerateTokensAsync(user, ipAddress, ct, rt);
        }

        public async Task RevokeTokenAsync(RevokeTokenRequest request, string ipAddress, CancellationToken ct = default)
        {
            var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.RefreshToken, ct);
            if (refreshToken == null || !refreshToken.IsActive)
            {
                throw new InvalidOperationException("Token not found or already revoked");
            }

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            await _db.SaveChangesAsync(ct);
        }

        private async Task<AuthResponse> GenerateTokensAsync(ApplicationUser user, string ipAddress, CancellationToken ct, RefreshToken? existingRefreshToken = null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, accessExpires) = _jwtFactory.CreateAccessToken(user, roles);

            RefreshToken refreshToken;
            if (existingRefreshToken == null)
            {
                var rt = _jwtFactory.CreateRefreshToken();
                refreshToken = new RefreshToken
                {
                    Token = rt.token,
                    Expires = rt.expiresAt,
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress,
                    UserId = user.Id
                };

                _db.RefreshTokens.Add(refreshToken);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                refreshToken = existingRefreshToken;
            }

            return new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpires,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.Expires
            };
        }

        public async Task DeleteUserAsync(string userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(";", result.Errors.Select(e => e.Description)));
            }

            // Optional: also delete their refresh tokens
            var tokens = _db.RefreshTokens.Where(r => r.UserId == user.Id);
            _db.RefreshTokens.RemoveRange(tokens);
            await _db.SaveChangesAsync(ct);
        }

    }
}
