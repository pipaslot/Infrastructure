using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pipaslot.SecurityUI.ActionAbstraction;
using Pipaslot.SecurityUI.Actions;

namespace Pipaslot.SecurityUI
{
    internal class Router<TKey>
    {
        private readonly HttpRequest _request;
        private readonly string _routePrefix;

        public Router(HttpRequest request, string routePrefix)
        {
            _request = request;
            _routePrefix = "/" + routePrefix.TrimStart('/');
        }

        public IAction ResolveAction()
        {
            //Assets
            if (MatchFile("assets", ".js"))
            {
                var assetName = _request.Path.Value.Substring($"{_routePrefix}/assets".Length);
                return new JavascriptAction(assetName);
            }
            if (MatchFile("assets", ".css"))
            {
                var assetName = _request.Path.Value.Substring($"{_routePrefix}/assets".Length);
                return new StyleAction(assetName);
            }

            //API
            if (Match("api/role"))
            {
                return new RoleJsonAction();
            }
            if (Match("api/resource-instance"))
            {
                _request.Query.TryGetValue("resource", out var resource);
                return new ResourceInstanceJsonAction(resource);
            }
            if (Match("api/resource"))
            {
                return new ResourceJsonAction();
            }
            if (Match("api/permission"))
            {
                _request.Query.TryGetValue("role", out var role);
                _request.Query.TryGetValue("resource", out var resource);
                _request.Query.TryGetValue("instance", out var identifier);
                return new PermissionJsonAction<TKey>(ChangeType<TKey>(role), resource, ChangeType<TKey>(identifier));
            }
            if (Match("api/privilege"))
            {
                _request.Query.TryGetValue("role", out var role);
                _request.Query.TryGetValue("resource", out var resource);
                _request.Query.TryGetValue("instance", out var identifier);
                _request.Query.TryGetValue("permission", out var permission);
                _request.Query.TryGetValue("isAllowed", out var isAllowedString);
                bool? isAllowed = null;
                if (isAllowedString.Equals("true")) isAllowed = true;
                else if (isAllowedString.Equals("false")) isAllowed = false;
                return new PrivilegeJsonAction<TKey>(ChangeType<TKey>(role), resource, ChangeType<TKey>(identifier), permission, isAllowed);
            }

            //Pages
            if (Match("role"))
            {
                _request.Query.TryGetValue("roleId", out var roleId);
                _request.Query.TryGetValue("roleName", out var roleName);
                return new TemplateAction(_routePrefix, "role", new Dictionary<string, object>
                {
                    { "roleId", roleId },
                    { "roleName", roleName }
                });
            }
            return new TemplateAction(_routePrefix, "index");
        }

        private bool Match(string path, string method = "GET")
        {
            var expectedPath = $"{_routePrefix}/{path}";
            return _request.Path.Value.StartsWith(expectedPath) && string.Equals(_request.Method, method, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool MatchFile(string path, string suffix)
        {
            var expectedPath = $"{_routePrefix}/{path}";
            return _request.Path.Value.StartsWith(expectedPath) && _request.Path.Value.EndsWith(suffix);
        }

        private T ChangeType<T>(StringValues obj)
        {
            var value = obj.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}