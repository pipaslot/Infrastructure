﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFramework.Entities;
using Pipaslot.Infrastructure.Security.Jwt;
using Pipaslot.Infrastructure.Security.JWT;

namespace Pipaslot.Demo.Api
{
    /// <summary>
    /// Authentication token creation
    /// </summary>
    [AllowAnonymous]
    [Area("Api")]
    [Route("api/[controller]")]
    public class SignController : Controller
    {
        private readonly JwtTokenManager _tokenManager;
        private readonly IRoleStore _roleStore;
        private readonly UserRepository _userRepository;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SignController(JwtTokenManager tokenManager, IRoleStore roleStore, UserRepository userRepository, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _tokenManager = tokenManager;
            _roleStore = roleStore;
            _userRepository = userRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        [HttpPost("register")]
        public Token Register(string username, string password)
        {
            var user = _userRepository.GetByLogin(username);
            if (user != null)
            {
                throw new AuthenticationException("Username already exists");
            }
            using (var uow = _unitOfWorkFactory.Create())
            {
                var systemRoles = _roleStore.GetSystemRoles<Role<int>>();
                var roles = systemRoles.Where(r => r.Type == RoleType.Guest || r.Type == RoleType.User).ToList();
                if (_userRepository.CreateQuery().GetTotalRowCount() == 0)
                {
                    //Creating first user which must be admin
                    var adminRole = systemRoles.First(r => r.Type == RoleType.Admin);
                    roles.Add(adminRole);
                }
                user = new User()
                {
                    Login = username,
                    PasswordHash = HashPassword(password)
                };
                user.UserRoles = roles.Select(r => new UserRole(user, r)).ToList();
                _userRepository.Insert(user);
                var identity = new UserIdentity(user.Id, roles);
                var token = _tokenManager.CreateToken(identity);

                uow.Commit();
                return token;
            }
        }
        /// <summary>
        /// Authenticate user and retursn authentication token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public Token Login(string username, string password)
        {
            var identity = GetIdentity(username, password);
            return _tokenManager.CreateToken(identity);
        }

        private UserIdentity GetIdentity(string username, string password)
        {
            var user = _userRepository.GetByLogin(username);
            if (user != null)
            {
                var passwordHash = HashPassword(password);
                if (user.PasswordHash == passwordHash)
                {
                    return new UserIdentity(user.Id, user.UserRoles.Select(ur=>ur.Role));
                }
            }
            throw new AuthenticationException("Bad User name or Password");

            //return new UserIdentity(user.Id, user.Roles);
        }
        /// <summary>
        /// Create password hash
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string HashPassword(string password)
        {
            var salt = "s!5s9wpvtskr5sgcw5r9x3shr5dja32rifzs3fjhd";
            var bytes = new UTF8Encoding().GetBytes(password + salt);
            byte[] hashBytes;
            using (var algorithm = new System.Security.Cryptography.SHA512Managed())
            {
                hashBytes = algorithm.ComputeHash(bytes);
            }
            return Convert.ToBase64String(hashBytes);
        }
    }
}