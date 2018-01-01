using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pipaslot.Infrastructure.Security.JWT;
using System.Linq;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security.Jwt
{
    public static class JwtAuthenticationExtensions
    {
        /// <summary>
        /// Configure Authentication and JwtBearer extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtTokenParameters"></param>
        public static void AddJwtAuthentication<TKey, TRole>(this IServiceCollection services, JwtTokenParameters jwtTokenParameters)
            where TRole : IRole
        {
            AddJwtAuthentication<TKey, TRole>(services, jwtTokenParameters, null, null);
        }

        /// <summary>
        /// Configure Authentication and JwtBearer extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtTokenParameters"></param>
        /// <param name="jwtSetup"></param>
        public static void AddJwtAuthentication<TKey, TRole>(this IServiceCollection services, JwtTokenParameters jwtTokenParameters,
            Action<JwtBearerOptions> jwtSetup)
            where TRole : IRole
        {
            AddJwtAuthentication<TKey, TRole>(services, jwtTokenParameters, null, jwtSetup);
        }

        /// <summary>
        /// Configure Authentication and JwtBearer extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtTokenParameters"></param>
        /// <param name="authSetup"></param>
        /// <param name="jwtSetup"></param>
        public static void AddJwtAuthentication<TKey, TRole>(this IServiceCollection services, JwtTokenParameters jwtTokenParameters, Action<AuthenticationOptions> authSetup, Action<JwtBearerOptions> jwtSetup = null)
            where TRole : IRole
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
                                var claims = token.Claims.ToList();
                                services.AddScoped<UserIdentity>(s => manager.CreateIdentity<TKey, TRole>(claims));
                                context.Principal = manager.CreatePrincipal(claims);
                                if (jwtTokenParameters.SendNewKeyInEveryResponse)
                                {
                                    var newToken = manager.CreateNewToken(claims);
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
