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
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Infra;
using Charlie.OpenIam.Web.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Charlie.OpenIam.Web.Controllers.Api
{
    /// <summary>
    /// 当前用户的控制器
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IamConfigurationDbContext _clientDbContext;
        private readonly IGeneralPermissionService _permissionService;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, IUserService userService,
            IamConfigurationDbContext clientDbContext, IGeneralPermissionService permissionService, IMapper mapper)
        {
            _userManager = userManager;
            _userService = userService;
            _clientDbContext = clientDbContext;
            _permissionService = permissionService;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取当前登陆用户的角色集合
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles")]
        public async Task<IEnumerable<string>> GetRoles()
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;

            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户未登录");
            }

            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            List<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value).ToList();
            }

            //var user = await _userManager.Users.AsNoTracking()
            //    .Include(itm => itm.UserOrganizations)
            //        .ThenInclude(itm => itm.Organization)
            //            .ThenInclude(itm => itm.OrganizationRoles)
            //                .ThenInclude(itm => itm.Role)
            //    .FirstOrDefaultAsync(itm => itm.Id == userId);

            //var userRoles = await _userManager.GetRolesAsync(user);
            //var deptRoles = user.UserOrganizations.Select(itm => itm.Organization)
            //    .SelectMany(itm => itm.OrganizationRoles)
            //    .Select(itm => itm.Role.Name);

            //var roleNames = deptRoles.Concat(userRoles).Distinct();

            var roles = await _userService.GetRolesAsync(userId, allowedClientIds: allowedClientIds);

            return roles?.Select(itm => itm.Name);
        }

        /// <summary>
        /// 获取当前登陆用户的视图权限集合
        /// </summary>
        /// <param name="clientId">目标 clientId</param>
        /// <param name="currentClientHost">当前发起调用的客户端地址。如果不是超级管理员，那么会以该地址对应的 client 作为目标 clientId。只有当未指定 clientId 时，会采用该值</param>
        /// <param name="treeView">是否返回树形结构</param>
        /// <returns></returns>
        [HttpGet("views")]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetMenus(string clientId = null, string currentClientHost = null, bool treeView = false)
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;

            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            List<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value).ToList();
            }

            if (allowedClientIds != null)
            {
                if (!String.IsNullOrWhiteSpace(clientId))
                {
                    if (!allowedClientIds.Contains(clientId))
                    {
                        throw new IamException(HttpStatusCode.BadRequest, "无权操作");
                    }
                    else
                    {
                        allowedClientIds.Clear();
                        allowedClientIds.Add(clientId);
                    }
                }
                else if (!String.IsNullOrWhiteSpace(currentClientHost))
                {
                    // 根据地址找到目标 client
                    var targetClient = _clientDbContext.Clients.SingleOrDefault(itm => EF.Functions.ILike(itm.ClientUri, $"%{currentClientHost}%"));
                    if (targetClient != null)
                    {
                        allowedClientIds.Clear();
                        allowedClientIds.Add(targetClient.ClientId);
                    }
                }
            }

            var result = await _userService.GetRolesAndPermissionsAsync(userId, allowedClientIds);
            return Ok(result.Permissions.Where(itm => itm.Type == PermissionType.View).Select(itm => Map(itm)).GetTreeLayout());
        }

        /// <summary>
        /// 获取当前登陆用户的角色权限集合
        /// </summary>
        /// <param name="currentClientHost">当前发起调用的客户端地址。如果不是超级管理员，那么会根据该地址返回对应的 client 所具有的权限</param>
        /// <returns></returns>
        [HttpGet("permissions")]
        public async Task<UserRolePermissionDto> GetRoleAndPermissions(string currentClientHost = null)
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;

            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            List<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value).ToList();
            }

            if (allowedClientIds != null && !String.IsNullOrWhiteSpace(currentClientHost))
            {
                // 根据地址找到目标 client
                var targetClient = _clientDbContext.Clients.SingleOrDefault(itm => EF.Functions.ILike(itm.ClientUri, $"%{currentClientHost}%"));
                if (targetClient != null)
                {
                    allowedClientIds.Clear();
                    allowedClientIds.Add(targetClient.ClientId);
                }
            }

            var result = await _userService.GetRolesAndPermissionsAsync(userId, allowedClientIds);

            return new UserRolePermissionDto
            {
                Roles = result.Roles.Select(itm => new RoleDto
                {
                    Id = itm.Id,
                    ClientId = itm.ClientId,
                    IsAdmin = itm.IsAdmin,
                    IsSuperAdmin = itm.IsSuperAdmin,
                    Name = itm.Name
                }),
                Permissions = result.Permissions.Select(itm => Map(itm)).GetTreeLayout()
            };
        }

        /// <summary>
        /// 获取当前登陆用户可见的组织机构树
        /// </summary>
        /// <returns></returns>
        [HttpGet("orgs")]
        public async Task<IEnumerable<OrganizationDto>> GetOrgs()
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;

            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户未登录");
            }

            var result = await _userService.GetOrgsAsync(userId);
            if (result == null)
            {
                return new List<OrganizationDto>();
            }

            return result;
        }

        /// <summary>
        /// 获取当前登陆用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<UserDto> GetUserInfo()
        {
            string userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;

            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户未登录");
            }

            var user = await _userService.GetAsync(userId);
            if (user == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "用户不存在");
            }

            return new UserDto
            {
                Id = user.Id,
                IdCard = user.IdCard,
                HomeAddress = user.HomeAddress,
                Phone = user.Phone,
                Username = user.Username,
                FirstName = user.FirstName,
                Position = user.Position,
                JobNo = user.JobNo,
                Gender = user.Gender,
                Organizations = user.Organizations
            };
        }

        /// <summary>
        /// 是否拥有权限
        /// </summary>
        /// <param name="permKey">权限 Key</param>
        /// <param name="isAdmin">是否要求是管理员</param>
        /// <returns></returns>
        [HttpGet("permcheck")]
        public async Task<bool> HasPermissionAsync(string permKey, bool isAdmin = false)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            List<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value).ToList();
            }

            return await _permissionService.HasPermissionAsync(User, permKey, isAdmin, allowedClientIds);
        }

        private PermissionDto Map(PermissionDto itm)
        {
            return new PermissionDto
            {
                Id = itm.Id,
                ClientId = itm.ClientId,
                Desc = itm.Desc,
                Key = itm.Key,
                Icon = itm.Icon,
                Level = itm.Level,
                Name = itm.Name,
                Order = itm.Order,
                ParentId = itm.ParentId,
                Type = itm.Type,
                Url = itm.Url,
                Children = itm.Children?.Select(child => Map(child)).ToList()
            };
        }
    }
}