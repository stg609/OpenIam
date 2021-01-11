using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Common.Helpers;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Areas.Admin.ViewModels;
using Charlie.OpenIam.Web.Helpers;
using Charlie.OpenIam.Web.Infra;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Charlie.OpenIam.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [UnitOfWork]
    [Area("Admin")]
    [Route("[area]/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(UserManager<ApplicationUser> userManager, IUserService userService, IMapper mapper)
        {
            _userMgr = userManager;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermission(BuiltInPermissions.USER_GET, true)]
        public async Task<PaginatedDto<AdminUserDto>> GetAllUsers(string firstName = null, string lastName = null, string jobNo = null, string idcard = null, string phone = null, string email = null, string excludeOrgId = null, int pageSize = 10, int pageIndex = 0, bool? isActive = null)
        {
            var users = await _userService.GetAllAsync(firstName, lastName, jobNo, idcard, phone, email, excludeOrgId, isActive, pageSize, pageIndex);

            return users;
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [HasPermission(BuiltInPermissions.USER_GET, true)]
        public async Task<ActionResult<AdminUserDetailsDto>> GetUser(string id)
        {
            return await _userService.GetAsync(id);
        }

        /// <summary>
        /// 获取用户的角色
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/roles")]
        [HasPermission(BuiltInPermissions.USER_GET, true)]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUserRoles(string id, bool getAll = false)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            var results = await _userService.GetRolesAsync(id, getAll, allowedClientIds);
            return Ok(results);
        }

        /// <summary>
        /// 获取用户的权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/permissions")]
        [HasPermission(BuiltInPermissions.USER_GET, true)]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetUserPermissions(string id, bool treeView = false)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            var roleAndPermissions = await _userService.GetRolesAndPermissionsAsync(id, allowedClientIds);

            return Ok(treeView ? roleAndPermissions.Permissions?.GetTreeLayout() : roleAndPermissions.Permissions);
        }

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HasPermission(BuiltInPermissions.USER_CREATE, true)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateUser(
            [FromServices] IOptionsSnapshot<IdentityOptions> identityOptions,
            UserNewViewModel model)
        {
            var id = await _userService.CreateAsync(_mapper.Map<UserNewDto>(model));
            return CreatedAtAction(nameof(GetUser), new { id = id }, new { id = id });
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [HasPermission(BuiltInPermissions.USER_UPDATE, true)]
        public async Task<ActionResult> UpdateUser(string id,
            [FromServices] IOptionsSnapshot<IdentityOptions> identityOptions,
            UserUpdateDto model)
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (userId == id && model.IsActive.HasValue &&!model.IsActive.Value)
            {
                throw new IamException(HttpStatusCode.BadRequest, "不能禁用自己！");
            }

            await _userService.UpdateAsync(id, model);
            return Ok();
        }

        /// <summary>
        /// 激活/冻结用户
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}/activity")]
        [HasPermission(BuiltInPermissions.USER_ACTIVE, true)]
        public async Task<ActionResult> InActive(string id, ActiveUserDto model)
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (userId == id && !model.IsActive)
            {
                throw new IamException(HttpStatusCode.BadRequest, "不能禁用自己！");
            }

            await _userService.SwitchAsync(id, model);
            return Ok();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [HasPermission(BuiltInPermissions.USER_DELETE, true)]
        public async Task<ActionResult> Delete(string id)
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (userId == id)
            {
                throw new IamException(HttpStatusCode.BadRequest, "不能删除自己！");
            }

            await _userService.RemoveAsync(id);
            return Ok();
        }

        /// <summary>
        /// 刷新密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.USER_PWD_RESET, true)]
        [HttpPost("{id}/pwd")]
        public async Task<ActionResult<string>> ResetPassword(string id)
        {
            return await _userService.ResetPwdAsync(id);
        }

        // 由于移除权限牵涉到较复杂的逻辑，因此通过单独的 Action 进行操作
        ///// <summary>
        ///// 修改某个用户权限（会覆盖原先的权限）
        ///// </summary>
        ///// <param name="id">用户编号</param>
        ///// <param name="permissions">修改后的权限</param>
        ///// <returns></returns>
        //[HasPermission(BuiltInPermissions.USER_ASSIGN_PERM, true)]
        //[HttpPut("{id}/permissions")]
        //public async Task<ActionResult> UpdatePermissions(string id, IEnumerable<UpdatePermissionToUserViewModel> permissions)
        //{
        //    var user = await _context.Users.Include(itm => itm.Permissions).SingleOrDefaultAsync(itm => itm.Id == id);
        //    if (user == null)
        //    {
        //        throw new IamException(HttpStatusCode.BadRequest, "用户不存在");
        //    }

        //    if (permissions != null && permissions.Any())
        //    {
        //        user.Permissions?.Clear();
        //        foreach (var permission in permissions)
        //        {
        //            user.Permissions.Add(new Models.UserPermission
        //            {
        //                PermissionId = permission.PermissionId,
        //                UserId = id,
        //                Action = permission.Action
        //            });
        //        }

        //        await _context.SaveChangesAsync();
        //    }

        //    return Ok();
        //}

        /// <summary>
        /// 赋予某个用户权限
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.USER_ASSIGN_PERM, true)]
        [HttpPost("{id}/permissions")]
        public async Task<ActionResult> AssignPermissions(string id, AssignPermissionToUserDto permissions)
        {
            if (permissions == null || permissions.PermissionIds == null || !permissions.PermissionIds.Any())
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

            await _userService.AssignPermissionsAsync(id, permissions, allowedClientIds);
            return Ok();
        }

        /// <summary>
        /// 移除某个用户权限
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.USER_ASSIGN_PERM, true)]
        [HttpDelete("{id}/permissions")]
        public async Task<ActionResult> RemovePermissions(string id, AssignPermissionToUserDto permissions)
        {
            if (permissions == null || permissions.PermissionIds == null)
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

            await _userService.RemovePermissionsAsync(id, permissions.PermissionIds, allowedClientIds);
            return Ok();
        }

        /// <summary>
        /// 赋予某个用户角色 （会替换原先的角色）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model">角色名称集合</param>7
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.USER_ASSIGN_ROLE, true)]
        [HttpPut("{id}/roles")]
        public async Task<ActionResult> AssignRoles(string id, AssignRoleToUserDto model)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _userService.AssignRolesAsync(id, model, allowedClientIds);
            return Ok();
        }
    }
}
