using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Infrastructure.Installers;
using System.Collections.Generic;
using System.Reflection;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string version = GetMajorVersion();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{version}", new Info
                {
                    Version = $"v{version}",
                    Title = "Artlogic TMS API",
                    Description = "API for Artlogic TMS"
                });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                });
                c.AddSecurityRequirement(security);

                c.IncludeXmlComments(GetXmlCommentsPath());
            });
            
            services.AddHttpContextAccessor();

            services.AddDomain(Configuration, true);
        }

        private static string GetXmlCommentsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Swagger.XML");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseMvc((routes) =>
            {
                routes.MapRoute(
                    name: "DefaultApi",
                    template: "api/{controller}/{action}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string version = GetMajorVersion();
                c.SwaggerEndpoint($"/swagger/v{version}/swagger.json", $"Artlogic TMS API v{version}");
            });
        }

        private string GetMajorVersion()
        {
            string versionString = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            return versionString;
        }
    }
}
