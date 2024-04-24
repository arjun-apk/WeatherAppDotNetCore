using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Context.User;
using WeatherApp.Models;

namespace WeatherApp.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JWTSettings _jwtSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<JWTSettings> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, userService, token).ConfigureAwait(false);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                // JWT token validation parameters
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = false,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

                // Attach user to context on successful JWT validation
                context.Items["User"] = await userService.GetUserById(userId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log the exception or return a proper error response
                // Do nothing if JWT validation fails
                // User is not attached to context so the request won't have access to secure routes
                Console.WriteLine($"JWT validation failed: {ex.Message}");
            }
        }
    }
}
