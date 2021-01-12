using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Core.Models.Repositories;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;

namespace Charlie.OpenIam.Core.Models.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IMapper _mapper;
        private readonly IPermissionRepo _permissionRepo;

        public PermissionService(IMapper mapper, IPermissionRepo permissionRepo)
        {
            _mapper = mapper;
            _permissionRepo = permissionRepo;
        }

        public async Task<string> AddAsync(PermissionNewDto permission)
        {
            if (await _permissionRepo.IsExistedAsync(clientId: permission.ClientId, key: permission.Key))
            {
                throw new IamException(HttpStatusCode.BadRequest, "已经存在");
            }

            if (permission.Type == PermissionType.View)
            {
                if (String.IsNullOrWhiteSpace(permission.Url))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "Url 不能为空");
                }
            }

            if (!String.IsNullOrWhiteSpace(permission.ParentId))
            {
                var parent = await _permissionRepo.GetAsync(permission.ParentId);
                if (parent == null)
                {
                    throw new IamException(HttpStatusCode.BadRequest, "父级不存在！");
                }

                if (parent.ClientId != permission.ClientId)
                {
                    throw new IamException(HttpStatusCode.BadRequest, $"父级并不属于客户端({permission.ClientId})！");
                }
            }

            string id = Guid.NewGuid().ToString();
            _permissionRepo.Add(new Permission(id, permission.ClientId, permission.Type, permission.Key, permission.Name, permission.Desc,
                permission.ParentId, permission.Url, permission.Icon, permission.Order, permission.Level));

            return id;
        }

        public async Task<IEnumerable<PermissionDto>> GetAllsync(string name = null, string key = null, string url = null, string targetClientId = null, PermissionType? type = null, IEnumerable<string> allowedClientIds = null)
        {
            var permissions = await _permissionRepo.GetAllAsync(name, key, url, targetClientId, type, allowedClientIds);
            return permissions?.Select(itm => new PermissionDto
            {
                Id = itm.Id,
                Key = itm.Key,
                Name = itm.Name,
                Desc = itm.Description,
                Type = itm.Type,
                ClientId = itm.ClientId,
                ParentId = itm.Parent == null ? "" : itm.Parent.Id,
                Url = itm.Url,
                Icon = itm.Icon,
                Level = itm.Level,
                Order = itm.Order
            });
        }

        public async Task<PermissionDto> GetAsync(string id, IEnumerable<string> allowedClientIds = null)
        {
            var perm = await _permissionRepo.GetAsync(id);

            if (allowedClientIds != null && !allowedClientIds.Contains(perm.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权访问");
            }

            return new PermissionDto
            {
                Id = perm.Id,
                Key = perm.Key,
                Name = perm.Name,
                Desc = perm.Description,
                Type = perm.Type,
                ClientId = perm.ClientId,
                ParentId = perm.Parent == null ? null : perm.Parent.Id,
                Url = perm.Url,
                Icon = perm.Icon,
                Level = perm.Level,
                Order = perm.Order
            };
        }

        public async Task UpdateAsync(string id, PermissionUpdateDto model, IEnumerable<string> allowedClientIds = null)
        {
            var perm = await _permissionRepo.GetAsync(id, isReadonly: false);

            if (allowedClientIds != null && allowedClientIds.Contains(perm.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            if (model.Type == PermissionType.View)
            {
                if (String.IsNullOrWhiteSpace(model.Url))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "Url 不能为空");
                }
            }

            if (!String.IsNullOrWhiteSpace(model.ParentId))
            {
                var parent = await _permissionRepo.GetAsync(model.ParentId);
                if (parent == null)
                {
                    throw new IamException(HttpStatusCode.BadRequest, "父级不存在！");
                }

                if (parent.ClientId != perm.ClientId)
                {
                    throw new IamException(HttpStatusCode.BadRequest, $"父级并不属于客户端({perm.ClientId})！");
                }
            }
            perm.Update(model.Name, model.Desc, model.Type, model.ParentId, model.Url, model.Icon, model.Order, model.Level);
        }

        public async Task RemoveAsync(string ids)
        {
            var targetIds = ids.Split(",")?.Select(itm => itm.Trim());
            if (targetIds == null)
            {
                return;
            }

            await _permissionRepo.RemoveAsync(targetIds);
        }

        public async Task SyncPermissionsAsync(SyncPermissionDto permissions, IEnumerable<string> allowedClientIds = null)
        {
            if (allowedClientIds!= null && !allowedClientIds.Contains(permissions.ClientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作！");
            }

            if (permissions.Permissions != null)
            {
                var syncPermKeys = permissions.Permissions.Select(itm => itm.Key);

                // 移除被删除的
                await _permissionRepo.RemoveAsync(excludeKeys: syncPermKeys, allowedClientIds: new[] { permissions.ClientId });

                // 增加新增的
                var exsitedPerms = await _permissionRepo.GetAllAsync(clientId: permissions.ClientId);
                var toAdd = syncPermKeys.Except(exsitedPerms.Select(itm => itm.Key));
                if (toAdd.Any())
                {
                    var toAddItms = permissions.Permissions.Where(itm => toAdd.Contains(itm.Key));
                    foreach (var permission in toAddItms)
                    {
                        if (permission.Type == PermissionType.View)
                        {
                            if (String.IsNullOrWhiteSpace(permission.Url))
                            {
                                throw new IamException(HttpStatusCode.BadRequest, "Url 不能为空");
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(permission.ParentId))
                        {
                            var parent = await _permissionRepo.GetAsync(permission.ParentId, isReadonly: true);
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
                        _permissionRepo.Add(new Models.Permission(id, permission.ClientId, permission.Type, permission.Key, permission.Name, permission.Desc, permission.ParentId, permission.Url, permission.Icon, permission.Order, permission.Level));
                    }
                }

                // 更新已有的
                foreach (var permission in permissions.Permissions.Where(itm => !toAdd.Contains(itm.Key)))
                {
                    var existed = await _permissionRepo.GetAsync(permission.Key, clientId: permission.ClientId, isReadonly: false);

                    if (!String.IsNullOrWhiteSpace(permission.ParentId))
                    {
                        var parent = await _permissionRepo.GetAsync(permission.ParentId, isReadonly: true);
                        if (parent == null)
                        {
                            throw new IamException(HttpStatusCode.BadRequest, $"权限{permission.Key}父级{permission.ParentId}不存在！");
                        }

                        if (parent.ClientId != permission.ClientId)
                        {
                            throw new IamException(HttpStatusCode.BadRequest, $"权限{permission.Key}父级{permission.ParentId}并不属于客户端({permission.ClientId})！");
                        }
                    }

                    existed.Update(permission.Name, permission.Desc, permission.Type,
                       permission.ParentId, permission.Url, permission.Icon, permission.Order, permission.Level);
                }
            }
            else
            {
                await _permissionRepo.RemoveAsync(allowedClientIds: new[] { permissions.ClientId });
            }
        }
    }
}
