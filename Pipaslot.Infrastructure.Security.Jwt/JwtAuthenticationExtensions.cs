using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pipaslot.Infrastructure.Security.JWT;
using System.Linq;

namespace Pipaslot.Infrastructure.Security.Jwt
{
    public static class JwtAuthenticationExtensions
    {
        /// <summary>
        /// Configure Authentication and JwtBearer extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtTokenParameters"></param>
        public static void AddJwtAuthentication(this IServiceCollection services, JwtTokenParameters jwtTokenParameters)
        {
            AddJwtAuthentication(services, jwtTokenParameters, null, null);
        }

        /// <summary>
        /// Configure Authentication and JwtBearer extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtTokenParameters"></param>
        /// <param name="jwtSetup"></param>
        public static void AddJwtAuthentication(this IServiceCollection services, JwtTokenParameters jwtTokenParameters,
            Action<JwtBearerOptions> jwtSetup)
        {
            AddJwtAuthentication(services, jwtTokenParameters, null, jwtSetup);
        }

        /// <summary>
        /// Configure Authentication and JwtBearer extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtTokenParameters"></param>
        /// <param name="authSetup"></param>
        /// <param name="jwtSetup"></param>
        public static void AddJwtAuthentication(this IServiceCollection services, JwtTokenParameters jwtTokenParameters, Action<AuthenticationOptions> authSetup, Action<JwtBearerOptions> jwtSetup = null)
        {
            services.AddScoped(s => new JwtTokenManager(jwtTokenParameters));
            services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    authSetup?.Invoke(o);
                })
                .AddJwtBearer(options =>
                {
                    options.Audience = jwtTokenParameters.Audience;
                    options.ClaimsIssuer = jwtTokenParameters.Issuer;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtTokenParameters.Issuer,
                        ValidAudience = jwtTokenParameters.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = jwtTokenParameters.GetSymetricSecurityKey(),
                        ValidateLifetime = true,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            // Add the access_token as a claim, as we may actually need it
                            if (context.SecurityToken is JwtSecurityToken token)
                            {
                                var manager = new JwtTokenManager(jwtTokenParameters);
                                var jwtIdentity = manager.CreateIdentity(token.Claims);
                                context.Principal.AddIdentity(jwtIdentity);
                                if (jwtTokenParameters.SendNewKeyInEveryResponse)
                                {
                                    var newToken = manager.CreateNewToken(token.Claims);
                                    context.Response.Headers.Add("tokenValue", newToken.Value);
                                    context.Response.Headers.Add("tokenExpiration", newToken.Expiration.ToString("s") + "Z");
                                }
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            return Task.CompletedTask;
                        }
                    };

                    jwtSetup?.Invoke(options);
                });
        }
    }
}
