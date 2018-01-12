using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pipaslot.Infrastructure.Security;
using Pipaslot.SecurityUI.ActionAbstraction;
using Pipaslot.SecurityUI.Actions;
using Pipaslot.SecurityUI.Privileges;

namespace Pipaslot.SecurityUI
{
    internal class Router<TKey>
    {
        private readonly HttpRequest _request;
        private readonly IUser _user;
        private readonly string _routePrefix;

        public Router(HttpRequest request, string routePrefix, IUser user)
        {
            _request = request;
            _user = user;
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
            if (MatchAndAuthorize("api/role"))
            {
                return new RoleJsonAction();
            }
            if (MatchAndAuthorize("api/resource-instance"))
            {
                _request.Query.TryGetValue("resource", out var resource);
                return new ResourceInstanceJsonAction(resource);
            }
            if (MatchAndAuthorize("api/resource"))
            {
                return new ResourceJsonAction();
            }
            if (MatchAndAuthorize("api/permission"))
            {
                _request.Query.TryGetValue("role", out var role);
                _request.Query.TryGetValue("resource", out var resource);
                _request.Query.TryGetValue("instance", out var identifier);
                return new PermissionJsonAction<TKey>(ChangeType<TKey>(role), resource, ChangeType<TKey>(identifier));
            }
            if (MatchAndAuthorize("api/privilege"))
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
            
            return new TemplateAction(_routePrefix, "index");
        }

        private bool MatchAndAuthorize(string path, string method = "GET")
        {
            var expectedPath = $"{_routePrefix}/{path}";
            var matches = _request.Path.Value.StartsWith(expectedPath) &&
                          string.Equals(_request.Method, method, StringComparison.CurrentCultureIgnoreCase);
            if (matches)
            {
                _user.CheckPermissionAsync(typeof(SecurityUIResource), SecurityUIPermissions.Access)
                    .GetAwaiter()
                    .GetResult();
            }
            
            return matches;
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