﻿using System;
using System.IO;
using Application.Services.Identity;
using Application.Services.Orders;
using Application.Services.Roles;
using Application.Services.Translations;
using Application.Services.Transportations;
using Application.Services.UserIdProvider;
using Application.Services.Users;
using DAL;
using Domain.Services.Identity;
using Domain.Services.Orders;
using Domain.Services.Roles;
using Domain.Services.Translations;
using Domain.Services.Transportations;
using Domain.Services.UserIdProvider;
using Domain.Services.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Artlogic TMC API",
                    Description = "API for Artlogic TMC"
                });
                c.IncludeXmlComments(GetXmlCommentsPath());
            });
            
            services.Add(new ServiceDescriptor(typeof(AppDbContext), typeof(AppDbContext),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(IIdentityService), typeof(IdentityService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(IUserIdProvider), typeof(UserIdProvider),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(IUsersService), typeof(UsersService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(IRolesService), typeof(RolesService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(IOrdersService), typeof(OrdersService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(ITransportationsService), typeof(TransportationsService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(ITranslationsService), typeof(TranslationsService),  ServiceLifetime.Scoped) );
            /*end of add service implementation*/
            
            services.AddHttpContextAccessor();
            var connectionString = Configuration.GetConnectionString("DefaultDatabase");


            var buildServiceProvider = services.AddEntityFrameworkNpgsql()
                .AddDbContext<AppDbContext>(options=>
                {
                    options.UseNpgsql(connectionString);
                })
                .BuildServiceProvider();
            
            var appDbContext = buildServiceProvider.GetService<AppDbContext>();

            appDbContext.DropDb();
            appDbContext.SaveChanges();
            appDbContext.Migrate(connectionString);
        }

        private static string GetXmlCommentsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Swagger.XML");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API V1");
            });
        }
    }
}
