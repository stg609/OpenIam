using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common.Helpers;
using Charlie.OpenIam.Sdk.Configurations;
using Charlie.OpenIam.Sdk.Services.Dtos;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Charlie.OpenIam.Sdk.Services
{
    /// <summary>
    /// Identity Server Api
    /// </summary>
    public class IamApi
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IamBasicOptions _options;
        private readonly ILogger<IamApi> _logger;

        public IamApi(HttpClient httpClient, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, IOptions<IamBasicOptions> options, ILogger<IamApi> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;

            // 因为 middleware 生命周期属于 singleton，所以目前直接使用 IOptions
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// 判断用户是否拥有权限
        /// </summary>
        /// <remarks>
        /// 注意，Access token 中必须包含 iamApi 这个 scope
        /// </remarks>
        /// <param name="permissionKey">权限的Key</param>
        /// <param name="isAdminRequired">是否要求是管理员</param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> HasPermissionAsync(string permissionKey, bool isAdminRequired = false)
        {
            var user = _httpContextAccessor.HttpContext.User;

            if (user != null && user.Identity.IsAuthenticated)
            {
                int statusCode = -1;
                try
                {
                    string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

                    if (accessToken == null)
                    {
                        return new ApiResult<bool>
                        {
                            IsSucceed = false,
                            Message = "Access Token 为空"
                        };
                    }
                    _httpClient.SetBearerToken(accessToken);

                    var resp = await _httpClient.GetAsync($"{_options.Authority.TrimEnd('/')}/api/user/permcheck?permkey={permissionKey}&isadmin={isAdminRequired}");
                    statusCode = (int)resp.StatusCode;

                    var result = await resp.WhenResponseSuccess(respStr =>
                    {
                        bool.TryParse(respStr, out bool hasPerm);
                        return hasPerm;
                    });

                    return new ApiResult<bool>
                    {
                        IsSucceed = true,
                        StatusCode = statusCode,
                        Data = result
                    };
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"调用 IamApi 的 {nameof(HasPermissionAsync)} 失败");
                    return new ApiResult<bool>
                    {
                        IsSucceed = false,
                        StatusCode = statusCode,
                        Message = ex.Message
                    };
                }
            }

            return new ApiResult<bool>
            {
                IsSucceed = true,
                StatusCode = 200,
                Data = false
            };
        }

        /// <summary>
        /// 获取当前登陆用户的基本信息
        /// </summary>
        /// <remarks>
        /// 注意，Access token 中必须包含 iamApi 这个 scope
        /// </remarks>
        /// <returns></returns>
        public async Task<ApiResult<UserBasicInfoDto>> GetCurrentUserBasicInfoAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;

            if (user != null && user.Identity.IsAuthenticated)
            {
                string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

                if (accessToken == null)
                {
                    return new ApiResult<UserBasicInfoDto>
                    {
                        IsSucceed = false,
                        Message = "Access Token 为空"
                    };
                }
                _httpClient.SetBearerToken(accessToken);

                string userId = user.FindFirst(JwtClaimTypes.Subject)?.Value;
                var resp = await _httpClient.GetAsync($"{_options.Authority.TrimEnd('/')}/api/user");
                var info = await resp.WhenResponseSuccess(resp => JsonConvert.DeserializeObject<UserBasicInfoDto>(resp));
                return new ApiResult<UserBasicInfoDto>
                {
                    IsSucceed = true,
                    Data = info
                };
            }

            return new ApiResult<UserBasicInfoDto>
            {
                IsSucceed = false,
                Message = "用户没有登录!"
            };
        }

        /// <summary>
        /// 获取用户的基本信息
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<UserBasicInfoDto>> GetUserBasicInfoAsync(string userId)
        {
            var resp = await _httpClient.GetAsync($"{_options.Authority.TrimEnd('/')}/api/user?userid={userId}");
            if (resp == null)
            {
                return new ApiResult<UserBasicInfoDto>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Data = null,
                };
            }

            var info = await resp.WhenResponseSuccess(resp => JsonConvert.DeserializeObject<UserBasicInfoDto>(resp));
            return new ApiResult<UserBasicInfoDto>
            {
                IsSucceed = true,
                Data = info
            };
        }

        /// <summary>
        /// 获取视图（菜单）权限
        /// </summary>
        /// <param name="treeView">是否以树状结构返回</param>
        /// <remarks>
        /// 注意，Access token 中必须包含 iamApi 这个 scope
        /// </remarks>
        /// <returns></returns>
        public async Task<ApiResult<IEnumerable<PermissionDto>>> GetViewPermissionsAsync(bool treeView = false)
        {
            string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            if (accessToken == null)
            {
                return new ApiResult<IEnumerable<PermissionDto>>
                {
                    IsSucceed = false,
                    Message = "Access Token 为空"
                };
            }

            _httpClient.SetBearerToken(accessToken);

            // 根据地址来判断访问的是哪个 Client
            var req = _httpContextAccessor.HttpContext.Request;
            string targetPath = $"{req.Scheme}://{req.Host}";

            int statusCode = -1;
            try
            {
                var resp = await _httpClient.GetAsync($"{_options.Authority.TrimEnd('/')}/api/user/views?currentclienthost={targetPath}&treeview={treeView}");
                statusCode = (int)resp.StatusCode;

                var result = await resp.WhenResponseSuccess(respStr =>
                {
                    return JsonConvert.DeserializeObject<IEnumerable<PermissionDto>>(respStr);
                });

                return new ApiResult<IEnumerable<PermissionDto>>
                {
                    IsSucceed = true,
                    StatusCode = (int)resp.StatusCode,
                    Data = result
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"调用 IamApi 的 {nameof(GetViewPermissionsAsync)} 失败");
                return new ApiResult<IEnumerable<PermissionDto>>
                {
                    IsSucceed = false,
                    StatusCode = statusCode,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取当前用户在对应客户端中所具有的角色及权限
        /// </summary>
        /// <remarks>
        /// 注意，Access token 中必须包含 iamApi 这个 scope
        /// </remarks>
        /// <returns></returns>
        public async Task<ApiResult<UserRolePermissionDto>> GetRoleAndPermissionsAsync()
        {
            string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            if (accessToken == null)
            {
                return new ApiResult<UserRolePermissionDto>
                {
                    IsSucceed = false,
                    Message = "Access Token 为空"
                };
            }

            _httpClient.SetBearerToken(accessToken);

            // 根据地址来判断访问的是哪个 Client
            var req = _httpContextAccessor.HttpContext.Request;
            string targetPath = $"{req.Scheme}://{req.Host}";

            int statusCode = -1;
            try
            {
                var resp = await _httpClient.GetAsync($"{_options.Authority.TrimEnd('/')}/api/user/permissions?currentClientHost={targetPath}");
                statusCode = (int)resp.StatusCode;

                var result = await resp.WhenResponseSuccess(respStr =>
                {
                    return JsonConvert.DeserializeObject<UserRolePermissionDto>(respStr);
                });

                return new ApiResult<UserRolePermissionDto>
                {
                    IsSucceed = true,
                    StatusCode = (int)resp.StatusCode,
                    Data = result
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"调用 IamApi 的 {nameof(GetRoleAndPermissionsAsync)} 失败");
                return new ApiResult<UserRolePermissionDto>
                {
                    IsSucceed = false,
                    StatusCode = statusCode,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 添加新用户
        /// </summary>
        /// <remarks>
        /// 注意，Access token 中必须包含 iamApi 这个 scope
        /// </remarks>
        /// <param name="model">用于新增用户的数据模型</param>
        /// <param name="accessToken">当前登陆用户的 access token，默认不需要提供，如果需要使用其他 access token 才需要提供</param>
        /// <returns></returns>
        public async Task<ApiResult<UserNewRespDto>> CreateUserAsync(UserNewDto model, string accessToken = null)
        {
            if (model == null)
            {
                return new ApiResult<UserNewRespDto>
                {
                    IsSucceed = false,
                    Message = "model 参数为空"
                };
            }

            if (accessToken == null)
            {
                accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

                if (accessToken == null)
                {
                    return new ApiResult<UserNewRespDto>
                    {
                        IsSucceed = false,
                        Message = "Access Token 为空"
                    };
                }
            }

            _httpClient.SetBearerToken(accessToken);

            int statusCode = -1;
            try
            {
                StringContent body = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync($"{_options.Authority.TrimEnd('/')}/admin/api/users", body);
                statusCode = (int)resp.StatusCode;

                var result = await resp.WhenResponseSuccess(resp =>
                {
                    return JsonConvert.DeserializeObject<UserNewRespDto>(resp);
                });

                return new ApiResult<UserNewRespDto>
                {
                    IsSucceed = true,
                    StatusCode = (int)resp.StatusCode,
                    Data = result
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"调用 IamApi 的 {nameof(CreateUserAsync)} 失败");
                return new ApiResult<UserNewRespDto>
                {
                    IsSucceed = false,
                    StatusCode = statusCode,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <remarks>
        /// 注意，Access token 中必须包含 iamApi 这个 scope
        /// </remarks>
        /// <param name="userId">用户编号</param>
        /// <param name="model">用于更新用户的数据模型</param>
        /// <param name="accessToken">当前登陆用户的 access token，默认不需要提供，如果需要使用其他 access token 才需要提供</param>
        /// <returns></returns>
        public async Task<ApiResult> UpdateUserAsync(string userId, UserUpdateDto model, string accessToken = null)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                return new ApiResult
                {
                    IsSucceed = false,
                    Message = "用户Id 不能为空"
                };
            }
            if (model == null)
            {
                return new ApiResult
                {
                    IsSucceed = true
                };
            }

            if (accessToken == null)
            {
                accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

                if (accessToken == null)
                {
                    return new ApiResult
                    {
                        IsSucceed = false,
                        Message = "Access Token 为空"
                    };
                }
            }

            _httpClient.SetBearerToken(accessToken);

            int statusCode = -1;
            try
            {
                StringContent body = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var resp = await _httpClient.PutAsync($"{_options.Authority.TrimEnd('/')}/admin/api/users/{userId}", body);
                statusCode = (int)resp.StatusCode;

                await resp.WhenResponseSuccess();

                return new ApiResult
                {
                    IsSucceed = true,
                    StatusCode = (int)resp.StatusCode
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"调用 IamApi 的 {nameof(UpdateUserAsync)} 失败");
                return new ApiResult
                {
                    IsSucceed = false,
                    StatusCode = statusCode,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <remarks>
        /// 注意，Access token 中必须包含 iamApi 这个 scope
        /// </remarks>
        /// <param name="userId">用户编号</param>
        /// <param name="accessToken">当前登陆用户的 access token，默认不需要提供，如果需要使用其他 access token 才需要提供</param>
        /// <returns></returns>
        public async Task<ApiResult> DeleteUserAsync(string userId, string accessToken = null)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                return new ApiResult
                {
                    IsSucceed = false,
                    Message = "用户Id 不能为空"
                };
            }

            if (accessToken == null)
            {
                accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

                if (accessToken == null)
                {
                    return new ApiResult
                    {
                        IsSucceed = false,
                        Message = "Access Token 为空"
                    };
                }
            }

            _httpClient.SetBearerToken(accessToken);

            int statusCode = -1;
            try
            {
                var resp = await _httpClient.DeleteAsync($"{_options.Authority.TrimEnd('/')}/admin/api/users/{userId}");
                statusCode = (int)resp.StatusCode;

                await resp.WhenResponseSuccess();

                return new ApiResult
                {
                    IsSucceed = true,
                    StatusCode = (int)resp.StatusCode
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"调用 IamApi 的 {nameof(DeleteUserAsync)} 失败");
                return new ApiResult
                {
                    IsSucceed = false,
                    StatusCode = statusCode,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 使用 Client Credential 方式获取 Token(并缓存起来)
        /// </summary>
        /// <param name="identityServerAuthority"></param>
        /// <param name="cacheKey"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="scope"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        internal async Task<string> GetTokenAsync(string identityServerAuthority,
            string cacheKey, string clientId, string clientSecret, string scope, ILogger logger = null)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out string token))
            {
                var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Address = identityServerAuthority,
                    Policy = new DiscoveryPolicy
                    {
                        RequireHttps = false
                    }
                });

                // request token
                var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,

                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = scope
                });
                if (tokenResponse.IsError)
                {
                    logger?.LogError($"Failed to get token From IdentityServer:{tokenResponse.Error}");
                    throw new HttpRequestException(tokenResponse.Error);
                }

                _memoryCache.Set(cacheKey, tokenResponse.AccessToken, TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 30));
                token = tokenResponse.AccessToken;
            }
            return token;
        }

        /// <summary>
        /// 同步权限
        /// </summary>
        /// <param name="model"></param>
        /// <param name="accessToken">当前登陆用户的 access token，默认不需要提供，如果需要使用其他 access token 才需要提供</param>
        /// <returns></returns>
        internal async Task<ApiResult> SyncPermissionsAsync(SyncPermissionsDto model, string accessToken = null)
        {
            if (model == null)
            {
                return new ApiResult
                {
                    IsSucceed = true
                };
            }

            if (accessToken == null)
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return new ApiResult
                    {
                        IsSucceed = false,
                        Message = "HttpContext 为空"
                    };
                }

                accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

                if (accessToken == null)
                {
                    return new ApiResult
                    {
                        IsSucceed = false,
                        Message = "Access Token 为空"
                    };
                }
            }

            _httpClient.SetBearerToken(accessToken);

            int statusCode = -1;
            try
            {
                StringContent body = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var resp = await _httpClient.PutAsync($"{_options.Authority.TrimEnd('/')}/admin/api/permissionbatch", body);
                statusCode = (int)resp.StatusCode;

                await resp.WhenResponseSuccess();

                return new ApiResult
                {
                    IsSucceed = true,
                    StatusCode = (int)resp.StatusCode
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"调用 IamApi 的 {nameof(SyncPermissionsAsync)} 失败");
                return new ApiResult
                {
                    IsSucceed = false,
                    StatusCode = statusCode,
                    Message = ex.Message
                };
            }
        }
    }
}
