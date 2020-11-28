using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DerMistkaefer.DvbLive.Backend.ServiceSetup
{
    /// <summary>
    /// Loader Functions for the Swagger Documentation
    /// </summary>
    public static class SwaggerDocumentationLoader
    {
        /// <summary>
        /// Add the Swagger Services to the DI-Container - Configured for DvbLive.
        /// </summary>
        /// <param name="services">Dependency Injection Container.</param>
        public static void AddSwaggerDvbLive(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var xmlFile = $"{currentAssembly.GetName().Name}.xml";
            var xmlDocumentationPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", GenerateApiInfo(currentAssembly));
                options.DocInclusionPredicate(IsDocumentedController);
                options.IncludeXmlComments(xmlDocumentationPath);
                options.CustomSchemaIds(x => x.FullName);
                options.TagActionsBy(DetermineApiGroupNames);
            });
        }

        /// <summary>
        /// Use Swagger in the Asp.Net Request Pipeline - Configured for DvbLive.
        /// </summary>
        /// <param name="appBuilder">The Builder for the Application Instanz.</param>
        public static void UseSwaggerDvbLive(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseSwagger();
            appBuilder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                c.RoutePrefix = string.Empty;
            });
        }

        private static OpenApiInfo GenerateApiInfo(Assembly assembly)
            => new OpenApiInfo
            {
                Title = GetProjectName(assembly),
                Description = GetProjectDescription(assembly) + GetProjectReleaseVersion(assembly),
                Version = "v1"
            };

        private static bool IsDocumentedController(string _, ApiDescription api)
            => !string.IsNullOrWhiteSpace(api.GroupName) || !string.IsNullOrWhiteSpace(DetermineControllerName(api));

        private static IList<string> DetermineApiGroupNames(ApiDescription api)
            => !string.IsNullOrWhiteSpace(api.GroupName)
                ? new List<string> { api.GroupName }
                : new List<string> { DetermineControllerName(api) };

        private static string DetermineControllerName(ApiDescription api)
            => ((ControllerActionDescriptor)api.ActionDescriptor).ControllerName;

        private static string GetProjectReleaseVersion(Assembly assembly)
        {
            var release = assembly.GetName().Version?.ToString();
            var compileDate = File.GetLastWriteTimeUtc(assembly.Location).ToLocalTime();

            return $"<br />`Release: {release} ({compileDate})`";
        }

        private static string GetProjectName(Assembly assembly)
            => $"{assembly.GetCustomAttributes<AssemblyProductAttribute>().FirstOrDefault()?.Product} Documentation";

        private static string GetProjectDescription(Assembly assembly)
        {
            var projectDescription = assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description;

            return (projectDescription != null ? $"<strong>Descripton</strong><br /> {projectDescription}<br /><br />" : "");
        }
    }
}
