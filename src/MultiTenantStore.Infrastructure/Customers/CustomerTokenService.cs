using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Infrastructure.Jwt;

namespace MultiTenantStore.Infrastructure.Customers;

public sealed class CustomerTokenService : ICustomerTokenService
{
    private readonly JwtOptions _options;

    public CustomerTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public CustomerAuthResponseDto GenerateToken(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured.");
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(
            _options.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, customer.Email),
            new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new(ClaimTypes.Email, customer.Email),
            new("customer_id", customer.Id.ToString()),
            new("customer_email", customer.Email),
            new("token_type", "customer")
        };

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

        return new CustomerAuthResponseDto
        {
            CustomerId = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
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