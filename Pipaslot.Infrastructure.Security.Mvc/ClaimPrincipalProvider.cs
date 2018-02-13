﻿using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.Infrastructure.Security.Mvc
{
    class ClaimsPrincipalProvider : IClaimsPrincipalProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public ClaimsPrincipalProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public ClaimsPrincipal GetClaimsPrincipal()
        {
            return _contextAccessor.HttpContext.User;
        }
    }
}
