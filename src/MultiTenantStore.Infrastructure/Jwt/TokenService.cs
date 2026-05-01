using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiTenantStore.Application.Auth.DTOs;
using MultiTenantStore.Application.Auth.Interfaces;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Infrastructure.Jwt;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AuthResponseDto GenerateToken(
        ApplicationUser user,
        IEnumerable<string> roles,
        Guid? storeId = null,
        string? storeSlug = null,
        string? storeRole = null)
    {
        if (string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured.");
        }

        var roleList = roles.ToList();
        var globalRole = roleList.FirstOrDefault() ?? string.Empty;

        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(TenantClaimTypes.UserId, user.Id.ToString())
        };

        foreach (var role in roleList)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (storeId is not null)
        {
            claims.Add(new Claim(TenantClaimTypes.StoreId, storeId.Value.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(storeSlug))
        {
            claims.Add(new Claim(TenantClaimTypes.StoreSlug, storeSlug));
        }

        if (!string.IsNullOrWhiteSpace(storeRole))
        {
            claims.Add(new Claim(TenantClaimTypes.StoreRole, storeRole));
        }

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            UserId = user.Id,
            StoreId = storeId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            GlobalRole = globalRole,
            StoreRole = storeRole,
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = GenerateRefreshToken(),
            ExpiresAt = expiresAt
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}