﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace V2
{
    public class SwaggerAuthorizationFilter : IDocumentFilter
    {
        private IServiceProvider _provider;

        public SwaggerAuthorizationFilter(IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            this._provider = provider;
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var http = this._provider.GetRequiredService<IHttpContextAccessor>();
            var auth = this._provider.GetRequiredService<IAuthorizationService>();
            var definitionsList = new Dictionary<string, Schema>();

            var descriptions = context.ApiDescriptionsGroups.Items.Where(group => group.GroupName == swaggerDoc.Info.Extensions["groupName"]).SelectMany(group => group.Items);

            foreach (var description in descriptions)
            {
                var authAttributes = description.ControllerAttributes()
                    .OfType<AuthorizeAttribute>()
                    .Union(description.ActionAttributes()
                        .OfType<AuthorizeAttribute>());

                // check if this action should be visible
                var notShowen = isForbiddenDueAnonymous(http, authAttributes) ||
                                isForbiddenDuePolicy(http, auth, authAttributes);

                var route = "/" + description.RelativePath.TrimEnd('/');
                var path = swaggerDoc.Paths[route];

                if (!notShowen)
                {
                    var schemaRegistrySettings = new SchemaRegistrySettings();
                    var schemaIdManager = new SchemaIdManager(schemaRegistrySettings.SchemaIdSelector);
                    foreach (var parameterDescription in description.ParameterDescriptions)
                    {
                        var typeToUse = (parameterDescription.Type.GetGenericArguments()?.SingleOrDefault() != null
                            ? parameterDescription.Type.GetGenericArguments().Single()
                            : parameterDescription.Type);
                        var definitionKey = schemaIdManager.IdFor(typeToUse);
                        if (definitionKey == null)
                            continue;
                        if (swaggerDoc.Definitions.ContainsKey(definitionKey))
                        {
                            var definitionValue = swaggerDoc.Definitions[definitionKey];
                            definitionsList.Add(definitionKey, definitionValue);
                        }
                    }
                    continue; // user passed all permissions checks
                }

                // remove method or entire path (if there are no more methods in this path)
                switch (description.HttpMethod)
                {
                    case "DELETE": path.Delete = null; break;
                    case "GET": path.Get = null; break;
                    case "HEAD": path.Head = null; break;
                    case "OPTIONS": path.Options = null; break;
                    case "PATCH": path.Patch = null; break;
                    case "POST": path.Post = null; break;
                    case "PUT": path.Put = null; break;
                    default: throw new ArgumentOutOfRangeException("Method name not mapped to operation");
                }

                if (path.Delete == null && path.Get == null &&
                    path.Head == null && path.Options == null &&
                    path.Patch == null && path.Post == null && path.Put == null)
                    swaggerDoc.Paths.Remove(route);
            }
            swaggerDoc.Definitions = definitionsList;
        }

        private static bool isForbiddenDuePolicy(
            IHttpContextAccessor http,
            IAuthorizationService auth,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            var policies = attributes
                .Where(p => !String.IsNullOrEmpty(p.Policy))
                .Select(a => a.Policy)
                .Distinct();
            return policies.Any(p => Task.Run(async () => await auth.AuthorizeAsync(http.HttpContext.User, p)).Result == false);
        }

        private static bool isForbiddenDueAnonymous(
            IHttpContextAccessor http,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            return attributes.Any() && !http.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}