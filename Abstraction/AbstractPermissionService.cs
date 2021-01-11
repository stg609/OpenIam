using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
using IdentityModel;

namespace Charlie.OpenIam.Abstraction
{
    public abstract class AbstractPermissionService : IGeneralPermissionService
    {
        public virtual async Task<bool> HasPermissionAsync(ClaimsPrincipal User, string permKey, bool isAdminRequired = false, IEnumerable<string> clientIds = null)
        {
            if (!String.IsNullOrWhiteSpace(permKey))
            {
                UserRolePermissionDto perms = null;
                if (User.FindFirst(JwtClaimTypes.Subject) != null)
                {
                    // 说明有用户参与
                    perms = await GetUserRolesAndPermissionsAsync(User, clientIds);
                }
                else
                {
                    // 可能是直接由Client Credential 方式获取的 token
                    // 此时 perm 直接以 claim 形式存在
                    var permClaim = User.FindFirst(Constants.CLIENT_CLAIM_PREFIX + permKey);
                    if (permClaim != null && !isAdminRequired)
                    {
                        // 由于 Client 并非 User，所以没有管理员之说
                        return true;
                    }
                }

                if (perms != null)
                {
                    bool isAdmin = false;
                    bool isSuperAdmin = false;

                    // 如果这个用户本来就有这个 claim，则也是 super
                    if (User.HasClaim(itm => itm.Type == Constants.SUPERADMIN_CLAIM_TYPE))
                    {
                        isAdmin = true;
                        isSuperAdmin = true;
                    }
                    else
                    {
                        // 自定义 ClaimsIdentity 用于保存当前用户所归属的所有 Client
                        ClaimsIdentity roleIdentity = new ClaimsIdentity("PermissionHandler");

                        if (perms.Roles != null)
                        {
                            foreach (var role in perms.Roles)
                            {
                                if (role.IsSuperAdmin && !isSuperAdmin)
                                {
                                    roleIdentity.AddClaim(new Claim(Constants.SUPERADMIN_CLAIM_TYPE, true.ToString()));
                                    isAdmin = true;
                                    isSuperAdmin = true;
                                }

                                if (role.IsAdmin && !isAdmin)
                                {
                                    isAdmin = true;
                                }

                                if (!String.IsNullOrWhiteSpace(role.ClientId))
                                {
                                    roleIdentity.AddClaim(new Claim(JwtClaimTypes.ClientId, role.ClientId));
                                }
                            }
                        }

                        if (!User.Identities.Any(itm => itm.AuthenticationType == "PermissionHandler"))
                        {
                            User.AddIdentity(roleIdentity);
                        }
                    }

                    // 如果是超级管理员，则有所有权限
                    if (isSuperAdmin)
                    {
                        return true;
                    }

                    if (perms.Permissions.Any(itm => itm.Key == permKey))
                    {
                        // 如果要求只能是管理员，则必须具有管理员的角色
                        if (!isAdminRequired || (isAdminRequired && isAdmin))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return false;
        }

        protected abstract Task<UserRolePermissionDto> GetUserRolesAndPermissionsAsync(ClaimsPrincipal user, IEnumerable<string> clientIds = null);

        public virtual Task<bool> SyncPermissionsAsync(string authority, string clientId, string clientSecret) => Task.FromResult(false);
    }
}
