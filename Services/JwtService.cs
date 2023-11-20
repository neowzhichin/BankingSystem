using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService
{
    public static string GenerateJwtToken(string username)
    {
        var secretKey = Appsettings.app(new string[] { "JWT", "Secret" });
        var issuer = Appsettings.app(new string[] { "JWT", "Issuer" });
        var audience = Appsettings.app(new string[] { "JWT", "Audience" });
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // Add other claims as needed
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Adjust as needed
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GetUniqueIdFromJWT(string token)
    {
        var secretKey = Appsettings.app(new string[] { "JWT", "Secret" });
        var issuer = Appsettings.app(new string[] { "JWT", "Issuer" });
        var audience = Appsettings.app(new string[] { "JWT", "Audience" });
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        SecurityToken validatedToken;
        ClaimsPrincipal principal;

        try
        {
            // Validate the token and get claims
            principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            // Retrieve the username from the "sub" (subject) claim
            return principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
        }
        return "";
    }
}