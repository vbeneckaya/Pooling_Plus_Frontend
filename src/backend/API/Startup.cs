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
using System.Text;
using Application.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Infrastructure.Logging;
using Domain.Services.UserIdProvider;
using Application.Services.UserIdProvider;
using Infrastructure.Translations;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = LoggerFactory.CreateLogger(Configuration, "API");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = SigningOptions.SignIssuer,
                        ValidAudience = SigningOptions.SignAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SigningOptions.SignKey)),
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });            
            
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

                c.IncludeXmlComments(GetXmlCommentsPath());
            });
            
            services.AddHttpContextAccessor();

            services.AddDomain(Configuration, true);

            services.SyncTranslations();

            services.AddScoped<IUserIdProvider, UserIdProvider>();
        }

        private static string GetXmlCommentsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Swagger.XML");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseAuthentication();
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

            lifetime.ApplicationStopped.Register(OnAppStopped);
        }

        public void OnAppStopped()
        {
            Log.CloseAndFlush();
        }

        private string GetMajorVersion()
        {
            string versionString = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            return versionString;
        }
    }
}
