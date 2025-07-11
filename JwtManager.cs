using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Handles JWT creation, validation, decoding, and refreshing.
/// </summary>
public class JwtManager
{
    private string _secretKey;
    private const int DefaultDurationMinutes = 60; // 1 hour

    /// <summary>
    /// Initializes JwtManager with an optional secret key.
    /// </summary>
    public JwtManager(string secretKey = null)
    {
        if (!string.IsNullOrEmpty(secretKey))
            _secretKey = secretKey;
    }

    /// <summary>
    /// Sets the secret key for signing tokens.
    /// </summary>
    public void SetSecretKey(string secretKey)
    {
        _secretKey = secretKey;
    }

    /// <summary>
    /// Encodes a payload into a JWT token.
    /// </summary>
    public string EncodePayload(Dictionary<string, object> payload, string secretKey = null, int? durationMinutes = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? _secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>();
        foreach (var kvp in payload)
        {
            claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
        }
        var expires = DateTime.UtcNow.AddMinutes(durationMinutes ?? DefaultDurationMinutes);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Decodes a JWT token and returns the payload as a dictionary.
    /// </summary>
    public Dictionary<string, object> DecodeToken(string token, string secretKey = null)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? _secretKey));
        var validations = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = false // Don't check expiry here
        };
        try
        {
            var principal = handler.ValidateToken(token, validations, out var validatedToken);
            var claims = principal.Claims;
            var dict = new Dictionary<string, object>();
            foreach (var claim in claims)
            {
                dict[claim.Type] = claim.Value;
            }
            return dict;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Invalid token: " + ex.Message);
        }
    }

    /// <summary>
    /// Verifies the validity and signature of a JWT token.
    /// </summary>
    public bool VerifyToken(string token, string secretKey = null)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? _secretKey));
        var validations = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        try
        {
            handler.ValidateToken(token, validations, out var validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Refreshes a JWT token by re-issuing it with a new expiry.
    /// </summary>
    public string RefreshToken(string token, string secretKey = null, int? durationMinutes = null)
    {
        var payload = DecodeToken(token, secretKey);
        // Remove exp, iat, nbf if present
        payload.Remove("exp");
        payload.Remove("iat");
        payload.Remove("nbf");
        return EncodePayload(payload, secretKey, durationMinutes);
    }

    /// <summary>
    /// Checks if a JWT token is expired.
    /// </summary>
    public bool IsTokenExpired(string token, string secretKey = null)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? _secretKey));
        var validations = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        try
        {
            handler.ValidateToken(token, validations, out var validatedToken);
            return false;
        }
        catch (SecurityTokenExpiredException)
        {
            return true;
        }
        catch
        {
            // Other errors mean invalid token, treat as expired for safety
            return true;
        }
    }
}