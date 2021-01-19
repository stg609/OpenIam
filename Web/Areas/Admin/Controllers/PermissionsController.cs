using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common.Helpers;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Areas.Admin.ViewModels;
using Charlie.OpenIam.Web.Helpers;
using Charlie.OpenIam.Web.Infra;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Charlie.OpenIam.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 权限管理
    /// </summary>
    [UnitOfWork]
    [Area("Admin")]
    [Route("[area]/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;

        public PermissionsController(IPermissionService permissionService,IMapper mapper)
        {
            _permissionService = permissionService;
            _mapper = mapper;
        }

        /// <summary>
        /// 新增权限(仅可用于当前的 Client）
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.PERM_CREATE, true)]
        [HttpPost]
        public async Task<ActionResult> AddPermission(PermissionNewViewModel permission)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);

                if (String.IsNullOrWhiteSpace(permission.ClientId))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "Client Id 不能为空");
                }

                if (!allowedClientIds.Contains(permission.ClientId))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "无权操作！");
                }
            }

            var id = await _permissionService.AddAsync(_mapper.Map<PermissionNewDto>(permission));
            return Ok();
        }

        /// <summary>
        /// 获取拥有的所有权限
        /// </summary>
        /// <param name="name">权限的名称</param>
        /// <param name="key">权限的 key </param>
        /// <param name="url">权限对应的链接（用于菜单类型的权限）</param>
        /// <param name="targetClientId">权限所属的 clientId</param>
        /// <param name="type">权限类型</param>
        /// <param name="treeView">是否以树形结构显示</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.PERM_GETALL, true)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions(string name = null, string key = null, string url = null, string targetClientId = null, PermissionType? type = null, bool treeView = false)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }
                      
            var results = await _permissionService.GetAllsync(name, key, url, targetClientId, type, allowedClientIds);

            if (treeView)
            {
                return Ok(results.GetTreeLayout());
            }
            else
            {
                return Ok(results);
            }
        }


        /// <summary>
        /// 获取权限详情
        /// </summary>
        /// <param name="id">权限编号</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.PERM_GETALL, true)]
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetPermissionDetails(string id)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            return await _permissionService.GetAsync(id, allowedClientIds);
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.PERM_UPDATE, true)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePermissions(string id, PermissionUpdateDto model)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _permissionService.UpdateAsync(id, model, allowedClientIds);

            return Ok();
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.PERM_DELETE, true)]
        [HttpDelete]
        public async Task<ActionResult> DeletePermissions(PermissionRemoveViewModel model)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }
                        
            await _permissionService.RemoveAsync(model.Ids);

            return Ok();
        }
    }
}
