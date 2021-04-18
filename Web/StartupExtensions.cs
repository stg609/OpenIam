using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Repositories;
using Charlie.OpenIam.Core.Models.Services;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Infra;
using Charlie.OpenIam.Infra.Repositories;
using Charlie.OpenIam.Web.Configurations;
using Charlie.OpenIam.Web.Infra;
using Charlie.OpenIam.Web.Infra.Mappers;
using Hellang.Middleware.ProblemDetails;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Charlie.OpenIam.Web
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x =>
               {
                   x.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.FullName);
               }));
            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters = null;//默认只能数字、字母、下划线
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

            })
              .AddRoles<ApplicationRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opts =>
            {
                opts.LoginPath = "/Identity/Account/Login";
                opts.LogoutPath = "/Identity/Account/Logout";

                opts.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = RedirectIfRequired,
                    OnRedirectToAccessDenied = RedirectIfRequired
                };
            });

            return services;

            Task RedirectIfRequired(RedirectContext<CookieAuthenticationOptions> ctx)
            {
                if (!ctx.Request.Path.Value.Contains("/api/"))
                {
                    ctx.Response.Redirect(ctx.RedirectUri);
                }
                return Task.FromResult(0);
            }
        }

        public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            // IdentityServer4 基础设置及配置
            var idSrv = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore<IamConfigurationDbContext>(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x =>
                        {
                            x.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.FullName);
                        });
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x =>
                        {
                            x.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.FullName);
                        });
                    };
                    //允许清理
                    options.EnableTokenCleanup = true;
                    //清理条数
                    options.TokenCleanupBatchSize = 1000;
                    //每隔多长时间清理一次 (单位)s
                    options.TokenCleanupInterval = 3600;
                });

            if (env.IsDevelopment())
            {
                idSrv.AddDeveloperSigningCredential();
            }
            else
            {
                idSrv.AddCertificate(configuration);
            }

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IamOptions iamOpt, DingTalkOptions dingOpt, WwOptions wwOpt)
        {
            // 如果想部署到 nginx 的 子目录中，比如 foo 这个目录，那此时 url 为 /foo/api/user，但是 .net core 处理时需要去掉 foo
            string pathBase = iamOpt.PathBase?.Trim();
            pathBase = String.IsNullOrWhiteSpace(iamOpt.PathBase) ? "" : "/" + pathBase.TrimStart('/');

            services.AddAuthentication()
                 .AddCookie()
                 .AddJwtBearer("Bearer", options =>
                 {
                     // OpenIam 本身也做为 Api Resource 提供服务，当第三方访问 OpenIam 的 Api 的时候需要对 Token 进行验证

                     options.Authority = iamOpt.Host;
                     options.RequireHttpsMetadata = false;

                     options.Audience = Constants.IAM_API_SCOPE;

                     if (iamOpt.ValidIssuers != null)
                     {
                         // OpenIam 可能从外网访问，也可能从内网访问，issuer 不同
                         options.TokenValidationParameters.ValidIssuers = iamOpt.ValidIssuers;
                     }
                 })
                 .AddDingTalk("钉钉登录", opts =>
                 {
                     opts.AppKey = dingOpt.AppKey;
                     opts.AppSecret = dingOpt.AppSecret;
                     opts.IncludeUserInfo = dingOpt.IncludeUserInfo;
                     opts.ClientId = dingOpt.ClientId;
                     opts.ClientSecret = dingOpt.ClientSecret;

                     opts.SignInScheme = IdentityConstants.ExternalScheme;

                     // 需要手动加上
                     opts.AuthorizationEndpoint = $"{pathBase}/Identity/Account/DingTalkLogin";

                     opts.Events.OnCreatingTicket = async ctx =>
                     {
                         string json = ctx.User.GetRawText();

                         // 如果 jobNumber 找不到，则可以认为用户不存在
                         await Task.CompletedTask;
                     };

                     opts.Events.OnRemoteFailure = async ctx =>
                     {
                         var tempDataProvider = ctx.HttpContext.RequestServices.GetRequiredService<ITempDataProvider>();

                         tempDataProvider.SaveTempData(ctx.HttpContext, new Dictionary<string, object>
                         {
                             { "ErrorMessage",ctx.Failure.Message }
                         });
                         ctx.Response.Redirect($"{pathBase}/Identity/Account/Login");
                         ctx.HandleResponse();

                         await Task.CompletedTask;
                     };
                 })
                 .AddWw("企业微信登录", opts =>
                 {
                     opts.ClientId = wwOpt.ClientId;
                     opts.ClientSecret = wwOpt.ClientSecret;
                     opts.AgentId = wwOpt.AgentId;

                     opts.SignInScheme = IdentityConstants.ExternalScheme;

                     opts.AuthorizationEndpoint = $"{pathBase}/Identity/Account/WwLogin";

                     opts.Events.OnCreatingTicket = async ctx =>
                     {
                         string json = ctx.User.GetRawText();

                         // 如果 jobNumber 找不到，则可以认为用户不存在
                         await Task.CompletedTask;
                     };

                     opts.Events.OnRemoteFailure = async ctx =>
                     {
                         var tempDataProvider = ctx.HttpContext.RequestServices.GetRequiredService<ITempDataProvider>();

                         tempDataProvider.SaveTempData(ctx.HttpContext, new Dictionary<string, object>
                         {
                             { "ErrorMessage",ctx.Failure.Message }
                         });
                         ctx.Response.Redirect($"{pathBase}/Identity/Account/Login");
                         ctx.HandleResponse();

                         await Task.CompletedTask;
                     };
                 });

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(GeneralProfile).GetTypeInfo().Assembly);
            services.AddHttpContextAccessor();
            services.AddAuthorization();
            services.AddCors();

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, OpenIamPermissionHandler>();
            services.AddSingleton<IGeneralPermissionService, OpenIamPermissionService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IOrgService, OrgService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISysService, SysService>();

            services.AddScoped(typeof(UnitOfWorkActionFilter), typeof(UnitOfWorkActionFilter));
            services.AddScoped<IUnitOfWork>(sp => sp.GetService<ApplicationDbContext>());

            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IRoleRepo, RoleRepo>();
            services.AddScoped<IClientRepo, ClientRepo>();
            services.AddScoped<IOrgRepo, OrgRepo>();
            services.AddScoped<IPermissionRepo, PermissionRepo>();
            services.AddScoped<ISysRepo, SysRepo>();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, string xmlFilePath)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "OpenIam Api",
                    Version = "v1",
                    License = new OpenApiLicense
                    {
                        Name = "MIT 许可证"
                    }
                });

                //接口注释
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                c.IncludeXmlComments(xmlFilePath);
            });

            return services;
        }

        public static IServiceCollection AddCustomConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("zh"),
                    new CultureInfo("en"),
                    new CultureInfo("en-US"),
                };
                options.DefaultRequestCulture = new RequestCulture("zh");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting 
                // numbers, dates, etc.

                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, 
                // i.e. we have localized resources for.

                options.SupportedUICultures = supportedCultures;
            });

            services.AddOptions<IamOptions>()
             .Bind(configuration.GetSection(nameof(IamOptions)))
             .ValidateDataAnnotations();

            services.AddOptions<DingTalkOptions>()
             .Bind(configuration.GetSection(nameof(DingTalkOptions)))
             .ValidateDataAnnotations();

            services.AddOptions<WwOptions>()
             .Bind(configuration.GetSection(nameof(WwOptions)))
             .ValidateDataAnnotations();
            return services;
        }

        public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddProblemDetails(ConfigureProblemDetails);
            return services;

            void ConfigureProblemDetails(ProblemDetailsOptions options)
            {
                // Only include exception details in a development environment. There's really no nee
                // to set this as it's the default behavior. It's just included here for completeness :)
                options.IncludeExceptionDetails = (ctx, ex) => env.IsDevelopment();

                options.Map<IamException>(ex =>
                {
                    return new StatusCodeProblemDetails((int)ex.StatusCode)
                    {
                        Type = ex.ErrCode,
                        Title = ex.Message,
                        Detail = ex.Message
                    };
                });
            }
        }

        public static IIdentityServerBuilder AddCertificate(this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            //ATT, 在 IIS 中运行时需要管理员权限才能正常读取 pfx
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            string certPath = Path.Combine(basePath, configuration.GetValue<string>("Certificate:Path"));
            Log.Information($"Add credential from {certPath}");
            builder.AddSigningCredential(new X509Certificate2(certPath, configuration.GetValue<string>("Certificate:Password")));
            return builder;
        }

        public static void SeedDataForUserAndRoles(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userservices = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                // 添加角色
                if (!applicationDbContext.Roles.Any(itm=>itm.IsAdmin))
                {
                    roleMgr.CreateAsync(new ApplicationRole(Constants.ROLES_ADMIN, null, true, true)).Wait();
                }

                if (!applicationDbContext.Roles.Any(itm=>itm.Name == Constants.ROLES_GUEST))
                {
                    roleMgr.CreateAsync(new ApplicationRole(Constants.ROLES_GUEST, null, false, false)).Wait();
                }

                // 添加用户
                if (!applicationDbContext.Users.Any())
                {
                    var user = new ApplicationUser("Admin", isActive: true);
                    var identityResult = userMgr.CreateAsync(user, "111111").Result;
                    if (identityResult.Succeeded)
                    {
                        userMgr.AddToRolesAsync(user, new[] { Constants.ROLES_ADMIN }).Wait();
                    }
                }

                // 添加内置的权限
                foreach (var field in typeof(BuiltInPermissions).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    var display = field.GetCustomAttribute<DisplayAttribute>();
                    string desc = null;
                    if (display != null)
                    {
                        desc = display.Name;
                    }

                    string key = field.GetRawConstantValue() as string;
                    string id = Guid.NewGuid().ToString();
                    if (!applicationDbContext.Permissions.Any(itm => itm.Key == key))
                    {
                        applicationDbContext.Permissions.Add(new Permission(id, null, PermissionType.Api, key, field.Name, desc));
                    }
                }

                if (!applicationDbContext.SysInfo.Any())
                {
                    // 添加系统信息, 默认情况手机号、工号唯一
                    applicationDbContext.SysInfo.Add(new Core.Models.SystemInfo(true, true, true, true, false));
                }

                applicationDbContext.SaveChanges();
            }
        }

        public static void SeedDataForIdentityServer(this IApplicationBuilder app, IConfiguration configuration)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configContext = scope.ServiceProvider.GetRequiredService<IamConfigurationDbContext>();

                //ClientSettings settings = configuration.GetSection(nameof(ClientSettings)).Get<ClientSettings>();

                ////初始数据
                //if (!configContext.Clients.Any())
                //{
                //    foreach (var client in MemoryConfig.GetClients(settings))
                //    {
                //        configContext.Clients.Add(client.ToEntity());
                //    }
                //    configContext.SaveChanges();
                //}

                if (!configContext.IdentityResources.Any())
                {
                    foreach (var resource in MemoryConfig.GetIdentityResources())
                    {
                        configContext.IdentityResources.Add(resource.ToEntity());
                    }
                    configContext.SaveChanges();
                }

                if (!configContext.ApiScopes.Any())
                {
                    foreach (var apiScope in MemoryConfig.GetApiScopes())
                    {
                        configContext.ApiScopes.Add(apiScope.ToEntity());
                    }
                    configContext.SaveChanges();
                }

                if (!configContext.ApiResources.Any())
                {
                    foreach (var resource in MemoryConfig.GetApiResources())
                    {
                        configContext.ApiResources.Add(resource.ToEntity());
                    }
                    configContext.SaveChanges();
                }
            }
        }

    }
}
