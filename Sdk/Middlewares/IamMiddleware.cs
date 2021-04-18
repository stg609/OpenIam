using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Common.Helpers;
using Charlie.OpenIam.Sdk.Configurations;
using Charlie.OpenIam.Sdk.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using static IdentityModel.OidcConstants;

namespace Charlie.OpenIam.Sdk.Middlewares
{
    /// <summary>
    /// 身份认证授权中间件
    /// </summary>
    public static class IamMiddlewareExtension
    {
        /// <summary>
        /// 增加统一身份认证授权功能
        /// </summary>
        /// <remarks>
        /// AddIam 内部已经调用了 AddAuthentication，不需要再重复调用
        /// </remarks>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddIam(this IServiceCollection services, Action<IamOptions> configureOptions)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.ConfigureNonBreakingSameSiteCookies();

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddMemoryCache();

            IamOptions opts = new IamOptions
            {
                GetClaimsFromUserInfoEndpoint = false,
                RequireHttpsMetadata = true
            };

            if (configureOptions != null)
            {
                configureOptions(opts);
            }

            services.AddOptions<IamBasicOptions>().Configure(config =>
            {
                config.Authority = opts.Authority;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                if (!String.IsNullOrWhiteSpace(opts.AccessDeniedPath))
                {
                    //403跳转页
                    o.AccessDeniedPath = new PathString(opts.AccessDeniedPath);
                }

                o.Events.OnRedirectToAccessDenied = RedirectIfRequired;
                o.Events.OnRedirectToLogin = RedirectIfRequired;
                o.Events.OnValidatePrincipal = async ctx =>
                {
                    var identity = (ClaimsIdentity)ctx.Principal.Identity;

                    var accessToken = ctx.Properties.GetTokenValue("access_token");
                    var refreshToken = ctx.Properties.GetTokenValue("refresh_token");

                    var expiresAtStr = ctx.Properties.GetTokenValue("expires_at");
                    var expiresAt = DateTime.ParseExact(expiresAtStr, "o", CultureInfo.InvariantCulture);

                    if (DateTime.Now < expiresAt)
                    {
                        return;
                    }

                    // ATT: 由于此处并非使用 HttpClientFactory 创建的实例，为避免 DNS 切换导致的错误，此处会对 Client 进行回收，
                    // 另外由于 token 超时导致的请求不会很频繁，所以对性能不会造成严重影响
                    using (var client = new HttpClient())
                    {
                        var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                        {
                            Address = opts.Authority
                        });

                        // 如果已经过期，则使用 refresh token 重新请求 access token
                        var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                        {
                            Address = disco.TokenEndpoint,
                            ClientId = opts.ClientId,
                            ClientSecret = opts.ClientSecret,
                            RefreshToken = refreshToken
                        });

                        if (!response.IsError)
                        {
                            bool result = ctx.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);
                            result = ctx.Properties.UpdateTokenValue("access_token", response.AccessToken);
                            string newExpiresAt = expiresAt.AddSeconds(response.ExpiresIn).ToUniversalTime().ToString("o");
                            result = ctx.Properties.UpdateTokenValue("expires_at", newExpiresAt);

                            await ctx.HttpContext.SignInAsync(ctx.Principal, ctx.Properties);
                        }
                        else
                        {
                            // 如果换取不到，则需要用户重新登录
                            ctx.RejectPrincipal();
                            await ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        }
                    }
                };

                // 对于 Api 的请求不应该 Redirect
                Task RedirectIfRequired(RedirectContext<CookieAuthenticationOptions> ctx)
                {
                    if (!ctx.Request.Path.Value.Contains("/api/"))
                    {
                        ctx.Response.Redirect(ctx.RedirectUri);
                    }
                    return Task.FromResult(0);
                }
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = opts.Authority;
                options.ClientId = opts.ClientId;
                options.ClientSecret = opts.ClientSecret;
                options.GetClaimsFromUserInfoEndpoint = opts.GetClaimsFromUserInfoEndpoint;
                options.ResponseType = ResponseTypes.Code;
                options.RequireHttpsMetadata = opts.RequireHttpsMetadata;

                // 确保使用 Code 的时候必须使用 Pkce
                options.UsePkce = true;

                // 必须是 true，否则 Client 端将无法从 Cookie 中得到 Access Token
                options.SaveTokens = true;

                if (opts.Scopes != null)
                {
                    options.Scope.Clear();
                    foreach (var scope in opts.Scopes)
                    {
                        options.Scope.Add(scope);
                    }
                }

                // 用于使用 Iam Identity Resource 相关接口
                if (!options.Scope.Contains("openid"))
                {
                    options.Scope.Add("openid");
                }

                if (!options.Scope.Contains(Constants.IAM_ID_SCOPE))
                {
                    options.Scope.Add(Constants.IAM_ID_SCOPE);
                }

                if (!options.Scope.Contains(Constants.IAM_API_SCOPE))
                {
                    options.Scope.Add(Constants.IAM_API_SCOPE);
                }

                // 用于获取 refresh token
                if (!options.Scope.Contains("offline_access"))
                {
                    options.Scope.Add("offline_access");
                }

                options.ClaimActions.MapUniqueJsonKey("saler", "saler");
            });
            services.AddHttpContextAccessor();

            services.AddSingleton<IGeneralPermissionService, SdkPermissionService>();
            services.AddHttpClient<IamApi>()
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    // 避免跳转导致隐藏了最初的 http 状态码
                    return new HttpClientHandler()
                    {
                        AllowAutoRedirect = false
                    };
                });

            return services;
        }

        /// <summary>
        /// 为 Api 服务增加统一身份认证授权的辅助功能 (如：IamApi)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddIamForApiServices(this IServiceCollection services, Action<IamApiOptions> configureOptions)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddMemoryCache();

            IamApiOptions opts = new IamApiOptions();
            if (configureOptions != null)
            {
                configureOptions(opts);
            }

            services.AddOptions<IamBasicOptions>().Configure(config =>
            {
                config.Authority = opts.Authority;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = opts.Authority;
                    options.RequireHttpsMetadata = false;

                    options.Audience = opts.Audience;
                });

            services.AddHttpContextAccessor();
            services.AddHttpClient<IamApi>()
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    // 避免跳转导致隐藏了最初的 http 状态码
                    return new HttpClientHandler()
                    {
                        AllowAutoRedirect = false
                    };
                });
            
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IGeneralPermissionService, SdkPermissionService>();

            return services;
        }
    }
}
