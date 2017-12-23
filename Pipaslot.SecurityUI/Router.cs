﻿using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.SecurityUI.ActionAbstraction;
using Pipaslot.SecurityUI.Actions;

namespace Pipaslot.SecurityUI
{
    internal class Router
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
            if (MatchFile("assets",".js"))
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
            if (Match("json/roles.json"))
            {
                return new RoleJsonAction();
            }
            if (Match("json/resources.json"))
            {
                return new ResourceJsonAction();
            }
            if (MatchFile("json/resource/", ".json"))
            {
                var resourceNameWithSuffix = _request.Path.Value.Substring($"{_routePrefix}/json/resource/".Length);
                var resourceName = resourceNameWithSuffix.Substring(0, resourceNameWithSuffix.Length-5);
                return new ResourceInstanceJsonAction(resourceName);
            }

            //Pages
            if (Match("page/role/"))
            {
                return new TemplateAction(_routePrefix, "role");
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
    }
}