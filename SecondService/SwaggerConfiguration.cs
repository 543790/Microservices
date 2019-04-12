﻿using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SecondService
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddCustomizedSwagger(this IServiceCollection services)
        {
            services.AddMvcCore().AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Azure Practices API",
                    Description = "To call one service from another service"
                });
                //c.AddSecurityDefinition("Basic", new BasicAuthScheme
                //{
                //    Description = "Authorization header using the Basic Authentication",
                //    Type = "basic"
                //});
                //c.DocumentFilter<SwaggerSecurityRequirementsDocumentFilter>();
                //Locate the XML file being generated by ASP.NET Core Service...
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //... and tell Swagger to use those XML comments.
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        //public static IServiceCollection AddGlobalExceptionServices(this IServiceCollection services)
        //{
        //    services.AddMvc(options =>
        //    {
        //        options.Filters.Add(typeof(HttpGlobalExceptionFilter));
        //        options.Filters.Add(typeof(ValidateModelStateFilterAttribute));

        //    }).AddControllersAsServices();
        //    return services;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="IDocumentFilter" />
        //public class SwaggerSecurityRequirementsDocumentFilter : IDocumentFilter
        //{
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="document"></param>
        //    /// <param name="context"></param>
        //    public void Apply(SwaggerDocument document, DocumentFilterContext context)
        //    {
        //        document.Security = new List<IDictionary<string, IEnumerable<string>>>
        //        {
        //        new Dictionary<string, IEnumerable<string>>
        //        {
        //            { "Basic", new string[]{ } }
        //        }
        //    };
        //    }
        //}

    }
}
