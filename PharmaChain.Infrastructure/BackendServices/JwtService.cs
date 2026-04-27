using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string GenerateToken(LoginResponse user)
    {
        var claims = new[]
        {
        new Claim("UserId", user.UserId.ToString()),                
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),

        new Claim(ClaimTypes.Name, user.FullName ?? ""),             
        new Claim("UserName", user.UserName ?? ""),

        new Claim("BranchId", user.Branch.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Branch.ToString()),

        new Claim(ClaimTypes.Role, user.Role ?? "")
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_settings.DurationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}