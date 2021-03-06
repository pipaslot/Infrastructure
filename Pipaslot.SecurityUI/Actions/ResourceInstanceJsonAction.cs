﻿using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Security;
using Pipaslot.SecurityUI.ActionAbstraction;

namespace Pipaslot.SecurityUI.Actions
{
    class ResourceInstanceJsonAction : AJsonAction
    {
        private readonly string _resourceName;

        public ResourceInstanceJsonAction(string resourceName)
        {
            _resourceName = resourceName;
        }

        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var manager = (IPermissionManager)services.GetService(typeof(IPermissionManager));
            if (manager == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IPermissionManager)} from Dependency Injection.");
            }
            return manager.GetAllResourceInstancesAsync(_resourceName).Result;
        }
    }
}
