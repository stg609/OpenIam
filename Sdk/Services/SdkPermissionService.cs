using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Sdk.Services.Dtos;
using Microsoft.Extensions.Logging;

namespace Charlie.OpenIam.Sdk.Services
{
    /// <summary>
    /// 服务于 Sdk 的 Permission Service
    /// </summary>
    public class SdkPermissionService : AbstractPermissionService
    {
        private readonly IamApi _api;
        private readonly ILogger<SdkPermissionService> _logger;
        private const int KEY_INDEX = 0;
        private const int DESC_INDEX = 2;

        public SdkPermissionService(IamApi api, ILogger<SdkPermissionService> logger)
        {
            _api = api;
            _logger = logger;
        }

        protected override async Task<UserRolePermissionDto> GetUserRolesAndPermissionsAsync(ClaimsPrincipal user, IEnumerable<string> clientIds = null)
        {
            // 问 IdentityServer 要当前登陆用户的权限
            var perms = await _api.GetRoleAndPermissionsAsync();

            if (!perms.IsSucceed)
            {
                _logger.LogWarning($"Iam Middleware 在查询当前用户具有的权限时候失败 ({perms.StatusCode}):{perms.Message}");
                return null;
            }

            return perms.Data;
        }

        public override async Task<bool> SyncPermissionsAsync(string authority, string clientId, string clientSecret)
        {
            // 遍历子系统中所有 HasPermissionAttribute 然后报告给 Iam
            var attributes = new List<HasPermissionAttribute>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    attributes.AddRange(type.GetCustomAttributes<HasPermissionAttribute>(false));

                    foreach (var mtd in type.GetMethods())
                    {
                        attributes.AddRange(mtd.GetCustomAttributes<HasPermissionAttribute>(false));
                    }
                }
            }

            SyncPermissionsDto model = new SyncPermissionsDto
            {
                ClientId = clientId,
                Permissions = new List<PermissionDto>()
            };

            foreach (var attr in attributes)
            {
                var permsInfo = attr.Policy.Split(Constants.ColonDelimiter);
                model.Permissions.Add(new PermissionDto
                {
                    ClientId = clientId,
                    Key = permsInfo[KEY_INDEX],
                    Name = permsInfo[KEY_INDEX],
                    Desc = permsInfo[DESC_INDEX],
                    Type = PermissionType.Api
                });
            }

            // 通过 client credentials 的方式获取用于同步的 token
            var token = await _api.GetTokenAsync(authority, nameof(SyncPermissionsAsync), clientId, clientSecret, Constants.IAM_API_SCOPE, _logger);
            var result = await _api.SyncPermissionsAsync(model, token);
            if (result.IsSucceed)
            {
                return true;
            }
            else
            {
                _logger.LogWarning($"子系统（{clientId})同步权限失败！{result.Message}");
                return false;
            }
        }
    }
}
