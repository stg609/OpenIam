using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Models.Repositories;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Charlie.OpenIam.Core.Models.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IOrgRepo _orgRepo;
        private readonly IRoleRepo _roleRepo;
        private readonly IPermissionRepo _permissionRepo;
        private readonly RoleManager<ApplicationRole> _roleMgr;

        public RoleService(IMapper mapper, IOrgRepo orgRepo, IPermissionRepo permissionRepo, IRoleRepo roleRepo, RoleManager<ApplicationRole> roleManager)
        {
            _mapper = mapper;
            _orgRepo = orgRepo;
            _roleRepo = roleRepo;
            _permissionRepo = permissionRepo;
            _roleMgr = roleManager;
        }

        public async Task AddAsync(RoleNewDto role, IEnumerable<string> allowedClientIds = null)
        {
            if (allowedClientIds != null && !allowedClientIds.Contains(role.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            if (await _roleRepo.IsExistedAsync(role.Name, role.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "Role 已经存在");
            }

            var result = await _roleMgr.CreateAsync(new ApplicationRole(role.Name, role.ClientId, role.IsAdmin));

            if (!result.Succeeded)
            {
                throw new IamException(HttpStatusCode.BadRequest, String.Join(";", result.Errors.Select(err => err.Description)));
            }
        }

        public async Task RemoveAsync(IEnumerable<string> targetIds, IEnumerable<string> allowedClientIds = null)
        {
            if (targetIds == null || !targetIds.Any())
            {
                return;
            }

            var existed = await _roleRepo.GetAllAsync(roleIds: targetIds, allowedClientIds: allowedClientIds, pageSize: 0);
            if (existed.Data != null)
            {
                foreach (var itm in existed.Data)
                {
                    await _roleMgr.DeleteAsync(itm);
                }
            }
        }

        public async Task UpdateAsync(string id, RoleUpdateDto role, IEnumerable<string> allowedClientIds = null)
        {
            var existed = await _roleRepo.GetAsync(id, isReadonly: false);

            if (existed == null)
            {
                throw new IamException(HttpStatusCode.NotFound, "角色不存在");
            }

            if (allowedClientIds != null && !allowedClientIds.Contains(existed.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作!");
            }

            if (!String.IsNullOrWhiteSpace(role.Name) && existed.Name != role.Name)
            {
                if (await _roleRepo.IsExistedAsync(role.Name, existed.ClientId))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "该名称的权限已经存在");
                }
            }

            existed.Update(role.Name, role.Desc, role.IsAdmin);
        }

        public async Task<PaginatedDto<RoleDto>> GetAllAsync(string name = null, string clientId = null, IEnumerable<string> roleIds = null, bool withPerms = false, IEnumerable<string> allowedClientIds = null, string excludeOrgId = null, IEnumerable<string> excludeRoleIds = null, int pageSize = 10, int pageIndex = 1)
        {
            var roles = await _roleRepo.GetAllAsync(name, clientId, roleIds, withPerms, allowedClientIds, excludeOrgId, pageSize, pageIndex);

            if (excludeRoleIds != null && excludeRoleIds.Any())
            {
                roles.Data = roles.Data.Where(itm => !excludeRoleIds.Contains(itm.Id));
            }

            PaginatedDto<RoleDto> result = new PaginatedDto<RoleDto>
            {
                Data = roles.Data?.Select(itm => new RoleDto
                {
                    Id = itm.Id,
                    ClientId = itm.ClientId,
                    Name = itm.Name,
                    Desc = itm.Description,
                    IsSuperAdmin = itm.IsSuperAdmin,
                    IsAdmin = itm.IsSuperAdmin ? true : itm.IsAdmin,
                    Permissions = Map(itm.Permissions, itm.ClientId),
                    CreatedAt = itm.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                }),
                Total = roles.Total,
                PageIndex = roles.PageIndex,
                PageSize = roles.PageSize
            };

            return result;
        }


        public async Task<RoleDto> GetAsync(string id, bool withPerms = false, IEnumerable<string> allowedClientIds = null)
        {

            ApplicationRole role = await _roleRepo.GetAsync(id, withPerms);

            if (allowedClientIds != null && allowedClientIds.Any() && !allowedClientIds.Contains(role.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            return new RoleDto
            {
                Id = role.Id,
                ClientId = role.ClientId,
                Name = role.Name,
                Desc = role.Description,
                IsSuperAdmin = role.IsSuperAdmin,
                IsAdmin = role.IsSuperAdmin ? true : role.IsAdmin,
                Permissions = Map(role.Permissions, role.ClientId),
                CreatedAt = role.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public async Task<IEnumerable<RolePermissionDto>> GetPermissionsAsync(string id, bool getAllPermissions = false, IEnumerable<string> allowedClientIds = null)
        {
            var role = await _roleRepo.GetAsync(id, true);
            if (role == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "角色不存在");
            }

            if (allowedClientIds != null && allowedClientIds.Any() && !allowedClientIds.Contains(role.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            if (!getAllPermissions)
            {
                return role.Permissions.Select(itm => new RolePermissionDto
                {
                    Id = itm.Permission.Id,
                    Key = itm.Permission.Key,
                    Name = itm.Permission.Name,
                    Icon = itm.Permission.Icon,
                    Type = itm.Permission.Type,
                    ParentId = itm.Permission.Parent?.Id,
                    IsOwned = true,
                    Description = itm.Permission.Description,
                    Url = itm.Permission.Url,
                    Order = itm.Permission.Order,
                    Level = itm.Permission.Level
                });
            }

            var perms = await _permissionRepo.GetAllAsync(allowedClientIds: allowedClientIds);
            var rolePerms = role.Permissions.Select(itm => itm.PermissionId);

            return perms.Select(itm => new RolePermissionDto
            {
                Id = itm.Id,
                Key = itm.Key,
                Name = itm.Name,
                Icon = itm.Icon,
                Type = itm.Type,
                ParentId = itm.Parent?.Id,
                IsOwned = rolePerms.Any(permId => permId == itm.Id),
                Description = itm.Description,
                Url = itm.Url,
                Order = itm.Order,
                Level = itm.Level,
            });
        }

        public async Task UpdatePermissionsAsync(string id, AssignPermissionDto model, IEnumerable<string> allowedClientIds = null)
        {
            var role = await _roleRepo.GetAsync(id, true, false);

            if (role == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "角色不存在");
            }

            if (allowedClientIds != null && allowedClientIds.Any() && !allowedClientIds.Contains(role.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            if (model != null && model.PermissionIds != null)
            {
                role.RemovePermissions();
                foreach (var permId in model.PermissionIds)
                {
                    role.AddPermissions(permId);
                }
            }
        }

        public async Task AddPermissionsAsync(string id, AssignPermissionDto model, IEnumerable<string> allowedClientIds = null)
        {
            if (model == null || model.PermissionIds == null || !model.PermissionIds.Any())
            {
                return;
            }
            var role = await _roleRepo.GetAsync(id, true, false);

            if (role == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "角色不存在");
            }

            if (allowedClientIds != null && allowedClientIds.Any() && !allowedClientIds.Contains(role.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            foreach (var permId in model.PermissionIds)
            {

                role.AddPermissions(permId);
            }
        }

        public async Task RemovePermissionsAsync(string id, AssignPermissionDto model, IEnumerable<string> allowedClientIds = null)
        {
            if (model == null || model.PermissionIds == null || !model.PermissionIds.Any())
            {
                return;
            }

            var role = await _roleRepo.GetAsync(id, true, false);

            if (role == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "角色不存在");
            }

            if (allowedClientIds != null && allowedClientIds.Any() && !allowedClientIds.Contains(role.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }


            role.RemovePermissions(model.PermissionIds);
        }

        private IEnumerable<PermissionDto> Map(IEnumerable<Models.RolePermission> permissions, string clientId)
        {
            if (permissions == null)
            {
                return null;
            }

            var query = permissions.Where(perm => perm.Permission != null);
            if (!String.IsNullOrWhiteSpace(clientId))
            {
                query = query.Where(perm => perm.Permission.ClientId == clientId);
            }

            return query.Select(perm => new PermissionDto
            {
                Id = perm.PermissionId,
                Key = perm.Permission.Key,
                Desc = perm.Permission.Description,
                Name = perm.Permission.Name,
                ClientId = perm.Permission.ClientId,
                ParentId = perm.Permission.Parent?.Id
            });
        }
    }
}
