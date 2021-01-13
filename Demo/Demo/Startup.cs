using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Sdk.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication1;

namespace WebApplication2
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
            services.AddControllersWithViews();
            services.AddRazorPages();

            //认证服务client配置信息
            var idSettings = Configuration.GetSection("IdentityServerSettings").Get<IdentityServerSettings>();

            services.AddIam(opts =>
            {
                opts.Authority = idSettings.Authority;
                opts.ClientId = idSettings.ClientId;
                opts.ClientSecret = idSettings.ClientSecret;
                opts.GetClaimsFromUserInfoEndpoint = idSettings.GetClaimsFromUserInfoEndpoint;
                opts.RequireHttpsMetadata = idSettings.RequireHttpsMetadata;
                opts.Scopes = idSettings.Scopes;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();

            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            try
            {
                //同步权限
                var idSettings = Configuration.GetSection("IdentityServerSettings").Get<IdentityServerSettings>();

                var permSrv = app.ApplicationServices.GetRequiredService<IGeneralPermissionService>();
                permSrv.SyncPermissionsAsync(idSettings.Authority, idSettings.ClientId, idSettings.ClientSecret).Wait();
            }
            catch (System.Exception)
            {
                // 如果 OpenIam 没有启动，则会报错
            }
        }
    }
}
