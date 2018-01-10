using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Jwt;

namespace Pipaslot.Infrastructure.Security.JWT
{
    public class JwtTokenManager
    {
        private readonly JwtTokenParameters _parameters;

        public JwtTokenManager(JwtTokenParameters parameters)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// Create new token from user unique identifier and his role unique identifiers
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public Token CreateNewToken(object userId, IEnumerable<IRole> roles)
        {

            var claims = Helpers.RolesToClaims(roles);
            claims.Add(new Claim(ClaimTypes.Name, userId.ToString()));
            return CreateNewToken(claims);
        }

        /// <summary>
        /// Create new token for claims
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public Token CreateNewToken(IEnumerable<Claim> claims)
        {
            var signingCredentials = new SigningCredentials(_parameters.GetSymetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature);
            var expiration = DateTime.UtcNow.AddMinutes(_parameters.ExpirationInMinutes);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _parameters.Issuer,
                Audience = _parameters.Audience,
                Subject = new ClaimsIdentity(claims, "JWT", ClaimTypes.Name, ClaimTypes.Role),
                Expires = expiration,
                SigningCredentials = signingCredentials
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);

            return new Token
            {
                Value = tokenHandler.WriteToken(stoken),
                Expiration = expiration
            };
        }

        public ClaimsIdentity CreateIdentity(IEnumerable<Claim> claims)
        {
            return new ClaimsIdentity(claims, "JWT", ClaimTypes.Name, ClaimTypes.Role);
        }

        public bool IsTokenValid(string token)
        {
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(token);
                return jwt.ValidTo >= DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }
    }
}
