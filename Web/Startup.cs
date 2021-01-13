using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using Charlie.OpenIam.Web.Configurations;
using Charlie.OpenIam.Web.Infra;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;

namespace Charlie.OpenIam.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _configuration = configuration;
        }

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;

                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit 
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            var iamOptions = _configuration.GetSection(nameof(IamOptions)).Get<IamOptions>();
            var dingTalkOptions = _configuration.GetSection(nameof(DingTalkOptions)).Get<DingTalkOptions>();
            var wwOptions = _configuration.GetSection(nameof(WwOptions)).Get<WwOptions>();
            services.AddCustomDbContext(_configuration)
                .AddCustomIdentity()
                .AddCustomIdentityServer(_configuration, _env)
                .AddCustomConfigurations(_configuration)
                .AddCustomAuthentication(iamOptions, dingTalkOptions, wwOptions)
                .AddCustomServices()
                .AddCustomProblemDetails(_env)
                .AddCustomSwagger(XmlCommentsFilePath);

            services.AddControllersWithViews(opts =>
            {
                opts.Filters.AddService<UnitOfWorkActionFilter>();
            })
                .AddNewtonsoftJson(options =>
                  {
                      // .Net Core 3 目前还无法简单的来避免循环解析，所以使用 Newtonsoft
                      options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                  }); 

            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IdentityModelEventSource.ShowPII = true;
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseProblemDetails();
            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenIam Api v1");
            });

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Add this before any other middleware that might write cookies
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            app.SeedDataForUserAndRoles();
            app.SeedDataForIdentityServer(_configuration);
        }

        private string XmlCommentsFilePath
        {
            get
            {
                var basePath = AppContext.BaseDirectory;
                var fileName = this.GetType().GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}
