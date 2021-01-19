using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
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

namespace Charlie.OpenIam.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [UnitOfWork]
    [Area("Admin")]
    [Route("[area]/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RolesController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        [HasPermission(BuiltInPermissions.ROLE_CREATE, true)]
        public async Task<ActionResult> AddRole(RoleNewViewModel role)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _roleService.AddAsync(_mapper.Map<RoleNewDto>(role), allowedClientIds);

            return Ok();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="model">角色的编号</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ROLE_DELETE, true)]
        [HttpDelete]
        public async Task<ActionResult> DeleteRole(RoleRemoveViewModel model)
        {
            if (model == null || model.Ids == null)
            {
                return Ok();
            }

            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _roleService.RemoveAsync(model.Ids, allowedClientIds);
            return Ok();
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="id">角色的编号</param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ROLE_UPDATE, true)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(string id, RoleUpdateDto role)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _roleService.UpdateAsync(id, role, allowedClientIds);
            return Ok();
        }

        /// <summary>
        /// 获取角色集合
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="clientId">目标 ClientId</param>
        /// <param name="withPerms">是否返回权限</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ROLE_GET, true)]
        [HttpGet]
        public async Task<ActionResult<PaginatedDto<RoleDto>>> GetRoles(string name = null, string clientId = null, bool withPerms = false, int pageSize = 10, int pageIndex = 1)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            var results = await _roleService.GetAllAsync(name, clientId, withPerms: withPerms, allowedClientIds: allowedClientIds, pageSize: pageSize, pageIndex: pageIndex);

            return Ok(new PaginatedDto<RoleDto>
            {
                Data = results.Data,
                Total = results.Total,
                PageIndex = results.PageIndex,
                PageSize = results.PageSize
            });
        }

        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="id">角色编号</param>
        /// <param name="withPerms">是否返回权限</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ROLE_GET, true)]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleDetails(string id, bool withPerms = false)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            return await _roleService.GetAsync(id, withPerms, allowedClientIds);
        }

        /// <summary>
        /// 获取角色下的所有权限
        /// </summary>
        /// <param name="id">角色编号</param>
        /// <param name="getAll">是否包含所有角色，（i.e. 包含当前用户没有的角色）</param>
        /// <param name="treeView">是否以树形视图显示</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ROLE_GET, true)]
        [HttpGet("{id}/permissions")]
        public async Task<ActionResult<IEnumerable<RolePermissionDto>>> GetRolePermissions(string id, bool getAll = false, bool treeView = false)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            var results = await _roleService.GetPermissionsAsync(id, getAll, allowedClientIds);
            return Ok(treeView ? results.GetTreeLayout() : results);
        }

        /// <summary>
        /// 保存角色关联的权限
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="model">权限id集合</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ROLE_PERM_UPDATE, true)]
        [HttpPut("{id}/permissions")]
        public async Task<ActionResult> AssignPermissions(string id, AssignPermissionDto model)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _roleService.UpdatePermissionsAsync(id, model, allowedClientIds);
            return Ok();
        }
    }
}