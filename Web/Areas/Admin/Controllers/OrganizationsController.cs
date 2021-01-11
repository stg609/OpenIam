using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Common.Helpers;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Helpers;
using Charlie.OpenIam.Web.Infra;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Charlie.OpenIam.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 组织机构管理
    /// </summary>
    [UnitOfWork]
    [Area("Admin")]
    [Route("[area]/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrgService _orgService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public OrganizationsController(IOrgService orgService)
        {
            _orgService = orgService;
        }

        /// <summary>
        /// 获取组织机构集合
        /// </summary>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_GET, true)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetOrganizations(string name = null, bool treeView = false)
        {
            var orgs = await _orgService.GetAllAsync(name);

            return Ok(treeView ? orgs.GetTreeLayout() : orgs);
        }

        /// <summary>
        /// 通过机构编号获取机构信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_GET, true)]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDto>> GetOrganizationById(string id)
        {
            var org = await _orgService.GetAsync(id);
            if (org == null)
            {
                return NotFound("组织不存在");
            }

            return org;
        }

        /// <summary>
        /// 通过机构编号获取机构的默认角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getAll">是否要获取所有可用的角色</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_GET, true)]
        [HttpGet("{id}/roles")]
        public async Task<ActionResult<IEnumerable<OrganizationRoleDto>>> GetRolesByOrgId(string id, bool getAll = false)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            return Ok(await _orgService.GetRolesAsync(id, getAll, allowedClientIds));
        }

        /// <summary>
        /// 新增机构
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_CREATE, true)]
        [HttpPost]
        public async Task<ActionResult> AddOrganization(OrganizationNewDto organization)
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户没有登陆！");
            }

            string id = _orgService.Add(organization, userId);

            //await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrganizationById), new
            {
                Id = id
            });
        }

        /// <summary>
        /// 更新机构基本信息
        /// </summary>
        /// <param name="id">组织编号</param>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_UPDATE, true)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrganization(string id, OrganizationUpdateDto organization)
        {
            await _orgService.UpdateAsync(id, organization);
            //await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 删除组织机构
        /// </summary>
        /// <param name="ids">机构编号，多个编号用,分隔</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_DELETE, true)]
        [HttpDelete("{ids}")]
        public async Task<ActionResult> DeleteOrganization(string ids)
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户没有登陆！");
            }

            await _orgService.RemoveAsync(userId, ids);

            return Ok();
        }

        /// <summary>
        /// 增加默认角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_ROLE_CREATE, true)]
        [HttpPost("{id}/roles")]
        public async Task<ActionResult> AddDefaultRole(string id, AssignRoleToOrgDto model)
        {
            if (model == null || model.RoleIds == null || !model.RoleIds.Any())
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

            await _orgService.AddDefaultRolesAsync(id, model, allowedClientIds);
            //await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 移除默认角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_ROLE_DELETE, true)]
        [HttpDelete("{id}/roles")]
        public async Task<ActionResult> DeleteDefaultRole(string id, IEnumerable<string> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
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

            await _orgService.DeleteDefaultRolesAsync(id, roleIds, allowedClientIds);

            //await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 更新默认角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_ROLE_UPDATE, true)]
        [HttpPut("{id}/roles")]
        public async Task<ActionResult> UpdateDefaultRole(string id, AssignRoleToOrgDto model)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _orgService.UpdateDefaultRolesAsync(id, model, allowedClientIds);

            //await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 获取当前机构的所有用户，（不含子机构的人员）
        /// </summary>
        /// <param name="id">机构编号</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_USER_GET, true)]
        [HttpGet("{id}/users")]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetOrgUsers(string id)
        {
            return await _orgService.GetUsersAsync(id);
        }

        /// <summary>
        /// 更新机构的用户（用新的用户覆盖原先的用户）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_USER_UPDATE, true)]
        [HttpPut("{id}/users")]
        public async Task<ActionResult> UpdateOrgUsers(string id, AssignUserToOrgDto model)
        {            
            await _orgService.UpdateUsersAsync(id, model);

            //await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 增加机构的用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_USER_ADD, true)]
        [HttpPost("{id}/users")]
        public async Task<ActionResult> AddOrgUsers(string id, AssignUserToOrgDto model)
        {
            if (model == null || model.UserIds == null || !model.UserIds.Any())
            {
                return Ok();
            }
            await _orgService.AddUsersAsync(id, model);

            //await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 从机构中移除用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.ORGS_USER_REMOVE, true)]
        [HttpDelete("{id}/users")]
        public async Task<ActionResult> RemoveOrgUsers(string id, IEnumerable<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return Ok();
            }

            await _orgService.RemoveUsersAsync(id, userIds);
            //await _context.SaveChangesAsync();

            return Ok();
        }
    }
}