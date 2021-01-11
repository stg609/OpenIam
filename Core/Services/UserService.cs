using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Common.Helpers;
using Charlie.OpenIam.Core.Models.Repositories;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Charlie.OpenIam.Core.Models.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly IUserRepo _userRepo;
        private readonly IRoleRepo _roleRepo;
        private readonly IOrgRepo _orgRepo;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, IUserRepo userRepo, IRoleRepo roleRepo, IOrgRepo orgRepo, IMapper mapper)
        {
            _userMgr = userManager;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _orgRepo = orgRepo;
            _mapper = mapper;
        }

        public async Task<PaginatedDto<AdminUserDto>> GetAllAsync(string firstName = null, string lastName = null, string jobNo = null, string idcard = null, string phone = null, string email = null, string excludeOrgId = null, bool? isActive = null, int pageSize = 10, int pageIndex = 0)
        {
            var users = await _userRepo.GetAllAsync(firstName, lastName, jobNo, idcard, phone, email, excludeOrgId, isActive, pageSize, pageIndex);

            return new PaginatedDto<AdminUserDto>
            {
                Data = users.Data?.Select(itm => new AdminUserDto
                {
                    Id = itm.Id,
                    IdCard = itm.IdCard,
                    Phone = itm.PhoneNumber,
                    Username = itm.UserName,
                    FirstName = itm.FirstName,
                    LastName = itm.LastName,
                    Position = itm.Position,
                    JobNo = itm.JobNo,
                    Gender = itm.Gender,
                    IsActive = itm.IsActive,
                    LastIp = itm.LastIp,
                    LastLoginAt = itm.LastLoginAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreatedAt = itm.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    Organizations = itm.UserOrganizations.Select(itm => itm.Organization.Name).ToList()
                }),
                Total = users.Total,
                PageIndex = users.PageIndex,
                PageSize = users.PageSize
            };
        }

        public async Task<AdminUserDetailsDto> GetAsync(string id)
        {
            var user = await _userRepo.GetAsync(id);
            if (user == null)
            {
                throw new IamException(System.Net.HttpStatusCode.NotFound, "用户不存在");
            }

            var roles = await _userMgr.GetRolesAsync(user);
            var result = _mapper.Map<AdminUserDetailsDto>(user);
            result.Roles = roles;

            return result;
        }

        public async Task<IEnumerable<UserRoleDto>> GetRolesAsync(string id, bool getAllRoles = false, IEnumerable<string> allowedClientIds = null)
        {
            var user = await _userRepo.GetAsync(id);
            if (user == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "用户不存在");
            }

            var roleNames = await _userMgr.GetRolesAsync(user);
            var userRoles = await _roleRepo.GetAllByNamesAsync(roleNames, allowedClientIds);

            var orgRoles = user.UserOrganizations.SelectMany(itm => itm.Organization.OrganizationRoles.Select(itm => itm.Role));
            if (allowedClientIds != null)
            {
                // 普通管理员只能看到有权限的 clientId
                orgRoles = orgRoles.Where(itm => allowedClientIds.Contains(itm.ClientId));
            }

            List<UserRoleDto> results = null;
            if (!getAllRoles)
            {
                results = userRoles.Select(itm => new UserRoleDto
                {
                    Id = itm.Id,
                    Name = itm.Name,
                    Desc = itm.Description,
                    IsAdmin = itm.IsAdmin,
                    IsSuperAdmin = itm.IsSuperAdmin,
                    IsOwned = true,
                }).ToList();

                // 增加组织中包含的角色
                results.AddRange(orgRoles.Select(itm => new UserRoleDto
                {
                    Id = itm.Id,
                    Name = itm.Name,
                    Desc = itm.Description,
                    IsAdmin = itm.IsAdmin,
                    IsSuperAdmin = itm.IsSuperAdmin,
                    IsOwned = true,
                    IsBelongToOrg = true
                }));
                return results.Distinct();
            }

            var allRoles = await _roleRepo.GetAllAsync(allowedClientIds: allowedClientIds, pageSize: 0);

            results = allRoles.Data?.Select(itm => new UserRoleDto
            {
                Id = itm.Id,
                Name = itm.Name,
                Desc = itm.Description,
                IsAdmin = itm.IsAdmin,
                IsSuperAdmin = itm.IsSuperAdmin,
                IsOwned = userRoles.Any(role => itm.Id == role.Id) || orgRoles.Any(role => itm.Id == role.Id),
                IsBelongToOrg = orgRoles.Any(role => itm.Id == role.Id)
            }).ToList();          

            return results.Distinct();
        }

        public async Task<UserRolePermissionDto> GetRolesAndPermissionsAsync(string userId, IEnumerable<string> allowedClientIds = null, bool includePermissionsInRole = false)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var user = await _userRepo.GetAsync(userId);
            var userRoles = await _userMgr.GetRolesAsync(user);
            var deptRoles = user.UserOrganizations.Select(itm => itm.Organization)
                .SelectMany(itm => itm.OrganizationRoles)
                .Select(itm => itm.Role.Name);

            var roleNames = deptRoles.Concat(userRoles).Distinct();
            var roles = await _roleRepo.GetAllByNamesAsync(roleNames, allowedClientIds);

            var rolePermissions = roles.SelectMany(itm => itm.Permissions).Where(itm => itm.Permission != null && (allowedClientIds == null || allowedClientIds.Contains(itm.Permission.ClientId) || String.IsNullOrWhiteSpace(itm.Permission.ClientId)))
                .OrderBy(itm => itm.Permission.Order)
                .ToList();

            var userIncludePermissions = user.UserPermissions?
                .Where(itm => itm.Permission != null && (allowedClientIds == null || allowedClientIds.Contains(itm.Permission.ClientId) || String.IsNullOrWhiteSpace(itm.Permission.ClientId)) && itm.Action == PermissionAction.Include)
                .OrderBy(itm => itm.Permission.Order)
                .Select(itm => itm.Permission).ToList();

            var userExcludePermssions = user.UserPermissions?
                .Where(itm => itm.Permission != null && (allowedClientIds == null || allowedClientIds.Contains(itm.Permission.ClientId) || String.IsNullOrWhiteSpace(itm.Permission.ClientId)) && itm.Action == PermissionAction.Exclude)
                .ToList();

            if (userExcludePermssions != null && userExcludePermssions.Any())
            {
                userIncludePermissions?.RemoveAll(itm => userExcludePermssions.Any(ex => ex.PermissionId == itm.Id));
                rolePermissions?.RemoveAll(itm => userExcludePermssions.Any(ex => ex.PermissionRoleIds.Contains(itm.RoleId) && ex.PermissionId == itm.PermissionId));
            }

            userIncludePermissions?.AddRange(rolePermissions.Select(itm => itm.Permission));
            userIncludePermissions = userIncludePermissions?.Distinct().ToList();

            return new UserRolePermissionDto
            {
                Roles = roles.Select(itm => new RoleDto
                {
                    Id = itm.Id,
                    IsAdmin = itm.IsAdmin,
                    IsSuperAdmin = itm.IsSuperAdmin,
                    ClientId = itm.ClientId,
                    Name = itm.Name,
                    Permissions = includePermissionsInRole ? itm.Permissions.Select(itm => new PermissionDto
                    {
                        Id = itm.PermissionId,
                        Key = itm.Permission.Key,
                        ClientId = itm.Permission.ClientId,
                        Name = itm.Permission.Name,
                        Desc = itm.Permission.Description,
                        Type = itm.Permission.Type,
                        Icon = itm.Permission.Icon,
                        Level = itm.Permission.Level,
                        Order = itm.Permission.Order,
                        ParentId = itm.Permission.Parent == null ? "" : itm.Permission.Parent.Id,
                        Url = itm.Permission.Url
                    }) : null
                }),

                Permissions = userIncludePermissions.Select(itm => new PermissionDto
                {
                    Id = itm.Id,
                    Key = itm.Key,
                    ClientId = itm.ClientId,
                    Name = itm.Name,
                    Desc = itm.Description,
                    Type = itm.Type,
                    Icon = itm.Icon,
                    Level = itm.Level,
                    Order = itm.Order,
                    ParentId = itm.Parent == null ? "" : itm.Parent.Id,
                    Url = itm.Url
                }).ToList()
            };
        }

        public async Task<string> CreateAsync(UserNewDto model)
        {
            var existed = await _userMgr.FindByNameAsync(model.Username);
            if (existed != null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "该用户名已经被使用！");
            }

            model.JobNo = model.JobNo?.Trim();

            if (!String.IsNullOrWhiteSpace(model.JobNo))
            {
                existed = await _userRepo.GetAsync(jobNo: model.JobNo);
                if (existed != null)
                {
                    throw new IamException(HttpStatusCode.BadRequest, "该工号用户已经存在！");
                }
            }

            var user = new ApplicationUser(model.Username, model.JobNo, model.Email, model.HomeAddress, model.IdCard, model.Phone, model.FirstName, model.LastName, model.Position, model.Gender, model.IsActive);

            IdentityResult result = await _userMgr.CreateAsync(user, model.Password ?? "111111");
            if (result.Succeeded)
            {
                if (!String.IsNullOrWhiteSpace(model.OrgIds))
                {
                    var orgIdColl = model.OrgIds.Split(",").Select(itm => itm.Trim());

                    foreach (var org in orgIdColl)
                    {
                        user.AddOrganizations(org);
                    }
                }

                await _userMgr.UpdateAsync(user);

                if (!String.IsNullOrWhiteSpace(model.JobNo))
                {
                    await _userMgr.AddClaimsAsync(user, new[]
                    {
                        new Claim(Constants.CLAIM_TYPES_SALER, model.JobNo)
                    });
                }
                return user.Id;
            }

            throw new IamException(HttpStatusCode.BadRequest, String.Join(";", result.Errors.Select(err => err.Description)));
        }

        public async Task UpdateAsync(string id, UserUpdateDto model)
        {
            var user = await _userRepo.GetAsync(id, isReadonly: false);
            if (user == null)
            {
                throw new IamException(HttpStatusCode.NotFound, "用户不存在");
            }

            user.Update(model.JobNo, model.Email, model.HomeAddress, model.IdCard, model.Phone, model.FirstName, model.LastName, model.Position, model.Gender, model.IsActive);


            if (!String.IsNullOrWhiteSpace(model.OrgIds))
            {
                var orgIdColl = model.OrgIds.Split(",").Select(itm => itm.Trim());
                user.RemoveOrganizations();

                foreach (var org in orgIdColl)
                {
                    user.AddOrganizations(org);
                }
            }

            IdentityResult result = await _userMgr.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (!String.IsNullOrWhiteSpace(model.JobNo))
                {
                    var claims = await _userMgr.GetClaimsAsync(user);
                    var salerClaim = claims.FirstOrDefault(itm => itm.Type == Constants.CLAIM_TYPES_SALER);
                    if (salerClaim != null)
                    {
                        await _userMgr.RemoveClaimAsync(user, salerClaim);
                    }
                    await _userMgr.AddClaimsAsync(user, new[]
                    {
                        new Claim(Constants.CLAIM_TYPES_SALER, model.JobNo)
                    });
                }
                return;
            }

            throw new IamException(HttpStatusCode.BadRequest, String.Join(";", result.Errors.Select(err => err.Description)));
        }

        public async Task SwitchAsync(string id, ActiveUserDto model)
        {
            var user = await _userRepo.GetAsync(id, isReadonly: false);
            if (user == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户不存在");
            }

            user.Switch(model.IsActive);
        }

        public async Task RemoveAsync(string id)
        {
            var user = await _userRepo.GetAsync(id, isReadonly: false);
            if (user == null)
            {
                throw new IamException(HttpStatusCode.NotFound, "用户不存在");
            }

            var result = await _userMgr.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new IamException(HttpStatusCode.BadRequest, String.Join(";", result.Errors.Select(err => err.Description)));
            }
        }

        public async Task<string> ResetPwdAsync(string id)
        {
            // 暂时不考虑邮件方式，只是简单的把密码返回
            var user = await _userRepo.GetAsync(id, isReadonly: false);
            if (user == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户不存在");
            }
            var token = await _userMgr.GeneratePasswordResetTokenAsync(user);
            var pwd = Helper.GetRandomString(8);

            var result = await _userMgr.ResetPasswordAsync(user, token, pwd);
            if (result.Succeeded)
            {
                return pwd;
            }

            throw new IamException(HttpStatusCode.BadRequest, String.Join(";", result.Errors.Select(err => err.Description)));
        }

        public async Task AssignPermissionsAsync(string id, AssignPermissionToUserDto permissions, IEnumerable<string> allowedClientIds = null)
        {
            if (permissions == null || permissions.PermissionIds == null || !permissions.PermissionIds.Any())
            {
                return;
            }

            var user = await _userRepo.GetAsync(id, isReadonly: false);
            if (user == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户不存在");
            }

            var ownedPermission = await GetRolesAndPermissionsAsync(id, allowedClientIds, true);

            foreach (var permission in permissions.PermissionIds)
            {
                var existed = user.UserPermissions.SingleOrDefault(itm => itm.PermissionId == permission);
                bool hasPermissionInRole = ownedPermission.Roles.Any(itm => itm.Permissions != null && itm.Permissions.Any(perm => perm.Id == permission));
                if (existed != null)
                {
                    if (existed.Action == PermissionAction.Include)
                    {
                        continue;
                    }
                    else
                    {
                        if (hasPermissionInRole)
                        {
                            // 如果角色中已经包含该权限，则只需要移除这个 exclude 权限即可。
                            user.RemovePermission(existed);
                        }
                        else
                        {
                            existed.Update(PermissionAction.Include);
                        }
                        continue;
                    }
                }

                if (!hasPermissionInRole)
                {
                    // 只有当角色中没有该权限时，才需要添加
                    user.AddPermission(permission, PermissionAction.Include);
                }
            }
        }

        public async Task RemovePermissionsAsync(string id, IEnumerable<string> permissionIds, IEnumerable<string> allowedClientIds = null)
        {
            if (permissionIds == null || !permissionIds.Any())
            {
                return;
            }

            var user = await _userRepo.GetAsync(id, isReadonly: false);
            if (user == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户不存在");
            }

            var ownedPermission = await GetRolesAndPermissionsAsync(id, allowedClientIds, true);

            foreach (var permission in permissionIds)
            {
                var existed = user.UserPermissions.SingleOrDefault(itm => itm.PermissionId == permission);
                IEnumerable<RoleDto> rolePerms = null;
                if (existed != null)
                {
                    // 如果是添加，则先删除
                    if (existed.Action == PermissionAction.Include)
                    {
                        user.RemovePermission(existed);
                    }
                    else
                    {
                        // 如果是排除，则检查下角色中是否有需要排除的
                        rolePerms = ownedPermission.Roles?.Where(itm => itm.Permissions != null && (itm.Permissions.Any(perm => perm.Id == permission)));
                        if (rolePerms.Any())
                        {
                            existed.Update(permissionRoleIds: rolePerms.Select(role => role.Id).ToArray());
                        }
                        continue;
                    }
                }

                // 如果其他角色中包含要排除的角色，则新建一个排除的权限
                rolePerms = ownedPermission.Roles?.Where(itm => itm.Permissions != null && (itm.Permissions.Any(perm => perm.Id == permission)));
                if (rolePerms.Any())
                {
                    user.AddPermission(permission, PermissionAction.Exclude, rolePerms.Select(role => role.Id).ToArray());
                }
            }
        }

        public async Task AssignRolesAsync(string id, AssignRoleToUserDto model, IEnumerable<string> allowedClientIds = null)
        {
            var user = await _userRepo.GetAsync(id, isReadonly: false);
            if (user == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户不存在");
            }
            var exsitedRoles = await _userMgr.GetRolesAsync(user);

            if (allowedClientIds != null)
            {
                var ownedRoles = await _roleRepo.GetAllByNamesAsync(exsitedRoles, allowedClientIds);
                exsitedRoles = exsitedRoles.Except(ownedRoles.Select(itm => itm.Name)).ToList();
            }

            IdentityResult result = await _userMgr.RemoveFromRolesAsync(user, exsitedRoles);
            if (!result.Succeeded)
            {
                throw new IamException(HttpStatusCode.BadRequest, String.Join(";", result.Errors.Select(err => err.Description)));
            }

            if (model == null || model.RoleIds == null || !model.RoleIds.Any())
            {
                return;
            }

            var allowedRoles = await _roleRepo.GetAllByIdsAsync(model.RoleIds, allowedClientIds);

            result = await _userMgr.AddToRolesAsync(user, allowedRoles.Select(itm => itm.Name));
            if (!result.Succeeded)
            {
                throw new IamException(HttpStatusCode.BadRequest, String.Join(";", result.Errors.Select(err => err.Description)));
            }
        }

        public async Task<IEnumerable<OrganizationDto>> GetOrgsAsync(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var user = await _userRepo.GetAsync(id);
            if (user == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "用户不存在");
            }

            if (user.UserOrganizations == null || !user.UserOrganizations.Any())
            {
                return Enumerable.Empty<OrganizationDto>();
            }

            var orgs = (await _orgRepo.GetAllAsync(isEnabled: true))
                           .Select(itm => _mapper.Map<OrganizationDto>(itm));

            var orgsInTree = orgs.GetTreeLayout();

            var orgIds = user.UserOrganizations.Select(itm => itm.OrganizationId);
            return orgsInTree.FilterTree(orgIds);
        }
    }
}
