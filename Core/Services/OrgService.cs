using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Core.Models.Repositories;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;

namespace Charlie.OpenIam.Core.Models.Services
{
    public class OrgService : IOrgService
    {
        private readonly IMapper _mapper;
        private readonly IOrgRepo _orgRepo;
        private readonly IRoleRepo _roleRepo;

        public OrgService(IMapper mapper, IOrgRepo orgRepo, IRoleRepo roleRepo)
        {
            _mapper = mapper;
            _orgRepo = orgRepo;
            _roleRepo = roleRepo;
        }

        public async Task<IEnumerable<OrganizationDto>> GetAllAsync(string name = null)
        {
            var orgs = await _orgRepo.GetAllAsync(name);
            if (orgs == null)
            {
                return Enumerable.Empty<OrganizationDto>();
            }

            return orgs.Select(itm => new OrganizationDto
            {
                Id = itm.Id,
                Address = itm.Address,
                Desc = itm.Desc,
                IsEnabled = itm.Enabled,
                Mobile = itm.Mobile,
                Name = itm.Name,
                ParentId = itm.Parent == null ? String.Empty : itm.Parent.Id,

                Roles = itm.OrganizationRoles.Select(role => new RoleDto
                {
                    Id = role.RoleId,
                    Name = role.Role.Name,
                    IsAdmin = role.Role.IsAdmin,
                    ClientId = role.Role.ClientId
                }).ToList(),

                CreatedAt = itm.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        public async Task<OrganizationDto> GetAsync(string id)
        {
            var org = await _orgRepo.GetAsync(id);
            return _mapper.Map<OrganizationDto>(org);
        }

        public async Task<IEnumerable<OrganizationRoleDto>> GetRolesAsync(string id, bool getAllRoles = false, IEnumerable<string> allowedClientIds = null)
        {
            var org = await _orgRepo.GetAsync(id);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "机构不存在");
            }

            if (!getAllRoles)
            {
                return org.OrganizationRoles.Select(itm => new OrganizationRoleDto
                {
                    Id = itm.RoleId,
                    Name = itm.Role.Name,
                    Desc = itm.Role.Description,
                    IsAdmin = itm.Role.IsAdmin,
                    IsSuperAdmin = itm.Role.IsSuperAdmin,
                    IsOwned = true,
                });
            }

            var roles = await _roleRepo.GetAllAsync(allowedClientIds: allowedClientIds, pageSize: 0);
            var orgRoles = org.OrganizationRoles.Select(itm => itm.RoleId);

            return roles.Data?.Select(itm => new OrganizationRoleDto
            {
                Id = itm.Id,
                Name = itm.Name,
                Desc = itm.Description,
                IsAdmin = itm.IsAdmin,
                IsSuperAdmin = itm.IsSuperAdmin,
                IsOwned = orgRoles.Any(roleId => itm.Id == roleId)
            });
        }

        public string Add(OrganizationNewDto organization, string userId)
        {
            string id = Guid.NewGuid().ToString();
            var org = new Organization(id, organization.Name, organization.Desc, organization.Address, organization.Mobile, organization.ParentId, organization.IsEnabled);

            org.AddUser(userId);
            _orgRepo.Add(org);

            return id;
        }

        public async Task UpdateAsync(string id, OrganizationUpdateDto organization)
        {
            var org = await _orgRepo.GetAsync(id, false);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.NotFound, "机构不存在");
            }

            org.Update(organization.Name, organization.Desc, organization.Address, organization.Mobile, organization.ParentId, organization.IsEnabled);
        }

        public async Task<IEnumerable<string>> RemoveAsync(string userId, IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Enumerable.Empty<string>();
            }

            return await _orgRepo.RemoveAsync(userId, ids);
        }

        public async Task AddDefaultRolesAsync(string id, AssignRoleToOrgDto model, IEnumerable<string> allowedClientIds = null)
        {
            var org = await _orgRepo.GetAsync(id, false);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "组织不存在");
            }

            if (allowedClientIds != null)
            {
                // 只能添加有权限添加的角色
                var roles = await _roleRepo.GetAllAsync(roleIds: model.RoleIds, allowedClientIds: allowedClientIds, pageSize: 0);
                model.RoleIds = roles.Data?.Select(itm => itm.Id);
            }

            if (model.RoleIds != null)
            {
                foreach (var itm in model.RoleIds)
                {
                    org.AddRole(itm);
                }
            }
        }

        public async Task DeleteDefaultRolesAsync(string id, IEnumerable<string> roleIds, IEnumerable<string> allowedClientIds = null)
        {
            if (roleIds == null || !roleIds.Any())
            {
                return;
            }

            var org = await _orgRepo.GetAsync(id, false);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "组织不存在");
            }

            if (allowedClientIds != null)
            {
                // 只能删除有权限删除的角色
                var roles = await _roleRepo.GetAllAsync(roleIds: roleIds, allowedClientIds: allowedClientIds, pageSize: 0);
                roleIds = roles.Data?.Select(itm => itm.Id);
            }
            org.RemoveDefaultRoles(itm => roleIds.Contains(itm.RoleId));
        }

        public async Task UpdateDefaultRolesAsync(string id, AssignRoleToOrgDto model, IEnumerable<string> allowedClientIds = null)
        {
            var org = await _orgRepo.GetAsync(id, false);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "组织不存在");
            }

            if (allowedClientIds != null)
            {
                org.RemoveDefaultRoles(itm => allowedClientIds.Contains(itm.Role.ClientId));
            }
            else
            {
                org.RemoveDefaultRoles();
            }

            if (model != null && model.RoleIds != null && model.RoleIds.Any())
            {
                if (allowedClientIds != null)
                {
                    var roles = await _roleRepo.GetAllAsync(roleIds: model.RoleIds, allowedClientIds: allowedClientIds, pageSize: 0);
                    model.RoleIds = roles.Data?.Select(itm => itm.Id);
                }

                foreach (var itm in model.RoleIds)
                {
                    org.AddRole(itm);
                }
            }
        }

        public async Task<List<AdminUserDto>> GetUsersAsync(string id)
        {
            var org = await _orgRepo.GetAsync(id, false);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "组织不存在");
            }

            List<AdminUserDto> users = new List<AdminUserDto>();
            foreach (var itm in org.UserOrganizations)
            {
                users.Add(_mapper.Map<AdminUserDto>(itm.User));
            }

            return users;
        }

        public async Task UpdateUsersAsync(string id, AssignUserToOrgDto model)
        {
            var org = await _orgRepo.GetAsync(id, false);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "组织不存在");
            }

            org.RemoveUsers();
            if (model == null || model.UserIds == null || !model.UserIds.Any())
            {
                return;
            }

            foreach (var itm in model.UserIds)
            {
                org.AddUser(itm);
            }
        }

        public async Task AddUsersAsync(string id, AssignUserToOrgDto model)
        {
            if (model == null || model.UserIds == null || !model.UserIds.Any())
            {
                return;
            }
            var org = await _orgRepo.GetAsync(id, false);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "组织不存在");
            }
            foreach (var itm in model.UserIds)
            {
                org.AddUser(itm);
            }
        }

        public async Task RemoveUsersAsync(string id, IEnumerable<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return;
            }

            var org = await _orgRepo.GetAsync(id, false);
            if (org == null)
            {
                throw new IamException(System.Net.HttpStatusCode.BadRequest, "组织不存在");
            }

            org.RemoveUsers(itm => userIds.Contains(itm.UserId));
        }
    }
}
