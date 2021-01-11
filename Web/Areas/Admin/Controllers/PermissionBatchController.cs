using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Core;
using Iam.ClientSdk;
using Iam.ClientSdk.Models;
using IdentityModel;
using IdentityServer.Areas.Admin.ViewModels;
using IdentityServer.Data;
using IdentityServer.Extensions;
using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Charlie.OpenIam.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 权限批量管理
    /// </summary>
    [Area("Admin")]
    [Route("[area]/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class PermissionBatchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public PermissionBatchController(ApplicationDbContext context, IStringLocalizer<Messages> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        /// <summary>
        /// 批量同步权限(仅可用于当前的 Client），会覆盖原先的权限
        /// </summary>
        /// <param name="permissions">需要同步的权限集合</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.PERM_SYNC)]
        [HttpPut]
        public async Task<ActionResult> SyncPermissions(SyncPermissionViewModel permissions)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);

                if (String.IsNullOrWhiteSpace(permissions.ClientId))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "Client Id 不能为空");
                }

                if (!allowedClientIds.Contains(permissions.ClientId))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "无权操作！");
                }
            }

            if (permissions.Permissions != null)
            {
                var syncPermKeys = permissions.Permissions.Select(itm => itm.Key);
                var toRemove = _context.Permissions.Where(itm => itm.ClientId == permissions.ClientId && !syncPermKeys.Contains(itm.Key));
                _context.Permissions.RemoveRange(toRemove);

                var toAdd = syncPermKeys.Except(_context.Permissions.Where(itm => itm.ClientId == permissions.ClientId).Select(itm => itm.Key));
                if (toAdd.Any())
                {
                    var toAddItms = permissions.Permissions.Where(itm => toAdd.Contains(itm.Key));
                    foreach (var permission in toAddItms)
                    {
                        if (_context.Permissions.Any(itm => itm.ClientId == permissions.ClientId && itm.Key == permission.Key))
                        {
                            throw new IamException(HttpStatusCode.BadRequest, $"权限{permission.Key}已经存在");
                        }

                        if (permission.Type == PermissionType.View)
                        {
                            if (String.IsNullOrWhiteSpace(permission.Url))
                            {
                                throw new IamException(HttpStatusCode.BadRequest, _localizer[Messages.UrlRequiredForViewPermission.ToString()]);
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(permission.ParentId))
                        {
                            var parent = await _context.Permissions.SingleOrDefaultAsync(itm => itm.Id == permission.ParentId);
                            if (parent == null)
                            {
                                throw new IamException(HttpStatusCode.BadRequest, $"权限{permission.Key}父级{permission.ParentId}不存在！");
                            }

                            if (parent.ClientId != permission.ClientId)
                            {
                                throw new IamException(HttpStatusCode.BadRequest, $"权限{permission.Key}父级{permission.ParentId}并不属于客户端({permission.ClientId})！");
                            }
                        }

                        string id = Guid.NewGuid().ToString();
                        _context.Permissions.Add(new Models.Permission(id, permission.ClientId, permission.Type, permission.Key, permission.Name, permission.Description,
                           permission.ParentId, permission.Url, permission.Icon, permission.Order, permission.Level));
                    }
                }

                // 更新已有的
                foreach (var permission in permissions.Permissions.Where(itm => !toAdd.Contains(itm.Key)))
                {
                    var existed = await _context.Permissions.SingleOrDefaultAsync(itm => itm.ClientId == permissions.ClientId && itm.Key == permission.Key);

                    if (!String.IsNullOrWhiteSpace(permission.ParentId))
                    {
                        var parent = await _context.Permissions.SingleOrDefaultAsync(itm => itm.Id == permission.ParentId);
                        if (parent == null)
                        {
                            throw new IamException(HttpStatusCode.BadRequest, $"权限{permission.Key}父级{permission.ParentId}不存在！");
                        }

                        if (parent.ClientId != permission.ClientId)
                        {
                            throw new IamException(HttpStatusCode.BadRequest, $"权限{permission.Key}父级{permission.ParentId}并不属于客户端({permission.ClientId})！");
                        }
                    }

                    existed.Update(permission.Name, permission.Description, permission.Type,
                       permission.ParentId, permission.Url, permission.Icon, permission.Order, permission.Level);
                }
            }
            else
            {
                var toClear = _context.Permissions.Where(itm => itm.ClientId == permissions.ClientId);
                _context.Permissions.RemoveRange(toClear);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
