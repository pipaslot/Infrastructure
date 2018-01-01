using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Jwt;

namespace Pipaslot.Infrastructure.Security.JWT
{
    public class JwtTokenManager
    {
        public const string TokenUserId = "uid";
        public const string TokenRole = "rol";
        private readonly JwtTokenParameters _parameters;

        public JwtTokenManager(JwtTokenParameters parameters)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// Create user from identity
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Token CreateToken(UserIdentity user)
        {
            var claims = new List<Claim>
            {
                new Claim(TokenUserId, JsonConvert.SerializeObject(user.Id)),
            };
            foreach (var role in user.Roles)
            {
                var claim = new Claim(TokenRole, JsonConvert.SerializeObject(role));
                claims.Add(claim);
            }
            return CreateNewToken(claims);
        }

        public Token CreateNewToken(IEnumerable<Claim> claims)
        {
            var signingCredentials = new SigningCredentials(_parameters.GetSymetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature);
            var expiration = DateTime.UtcNow.AddMinutes(_parameters.ExpirationInMinutes);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _parameters.Issuer,
                Audience = _parameters.Audience,
                Subject = new ClaimsIdentity(claims, "JWT", TokenUserId, TokenRole),
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

        public UserIdentity CreateIdentity<TKey, TRole>(IEnumerable<Claim> claims)
            where TRole : IRole
        {
            var idClaim = claims.FirstOrDefault(c => c.Type == TokenUserId)?.Value;
            var id = string.IsNullOrWhiteSpace(idClaim) ? default(TKey) : JsonConvert.DeserializeObject<TKey>(idClaim);
            var roles = claims
                .Where(c => c.Type == TokenRole)
                .Select(c => (IRole)JsonConvert.DeserializeObject<TRole>(c.Value))
            .ToList();
            return new UserIdentity(id, roles);
        }

        public ClaimsPrincipal CreatePrincipal(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, "JWT", TokenUserId, TokenRole);
            return new ClaimsPrincipal(identity);
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
