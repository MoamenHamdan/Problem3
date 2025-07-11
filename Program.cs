using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        var secret = "mySuperSecretKey12345678901234567890";
        var jwtManager = new JwtManager(secret);

        // 1. Encode payload
        var payload = new Dictionary<string, object>
        {
            { "userId", "123" },
            { "role", "admin" }
        };
        var token = jwtManager.EncodePayload(payload);
        Console.WriteLine("Generated Token: " + token);

        // 2. Decode token
        var decoded = jwtManager.DecodeToken(token);
        Console.WriteLine("Decoded Payload: " + string.Join(", ", decoded.Select(kv => kv.Key + "=" + kv.Value)));

        // 3. Verify token
        var isValid = jwtManager.VerifyToken(token);
        Console.WriteLine("Is token valid? " + isValid);

        // 4. Check if token is expired
        var expired = jwtManager.IsTokenExpired(token);
        Console.WriteLine("Is token expired? " + expired);

        // 5. Refresh token
        var refreshed = jwtManager.RefreshToken(token);
        Console.WriteLine("Refreshed Token: " + refreshed);
    }
}
