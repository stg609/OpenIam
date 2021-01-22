using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 角色实体
    /// </summary>
    public class ApplicationRole : IdentityRole, IAuditable, ISoftDeletable
    {
        /// <summary>
        /// 所属客户端id
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 是否是管理员 (只能管理所属 client 下的资源）
        /// </summary>
        public bool IsAdmin { get; private set; }

        /// <summary>
        /// 是否是超级管理员 (可管理所有 client)，如果 IsSuperAdmin = true 则可以忽略 IsAdmin
        /// </summary>
        public bool IsSuperAdmin { get; private set; }

        /// <summary>
        /// 角色权限
        /// </summary>
        public IReadOnlyCollection<RolePermission> Permissions => _permissions;
        private readonly List<RolePermission> _permissions = new List<RolePermission>();

        /// <summary>
        /// 组织角色
        /// </summary>
        public IReadOnlyCollection<OrganizationRole> OrganizationRoles => _organizationRoles;
        private readonly List<OrganizationRole> _organizationRoles = new List<OrganizationRole>();

        public string CreatedBy
        {
            get; private set;
        }

        public DateTime CreatedAt
        {
            get; private set;
        }

        public string LastUpdatedBy
        {
            get; private set;
        }

        public DateTime LastUpdatedAt
        {
            get; private set;
        }

        protected ApplicationRole()
        {

        }

        public ApplicationRole(string name, string clientId, bool isAdmin, bool isSuperAdmin = false)
        {
            Name = name;
            ClientId = clientId;
            IsAdmin = isAdmin;
            IsSuperAdmin = isSuperAdmin;
        }

        public void Update(string name, string desc, bool? isAdmin)
        {
            Name = name ?? Name;
            Description = desc ?? Description;
            IsAdmin = isAdmin ?? IsAdmin;
        }

        public void AddPermissions(string permId)
        {
            if(_permissions.Any(itm=>itm.PermissionId == permId))
            {
                return;
            }

            _permissions.Add(new RolePermission(Id, permId));
        }

        public void RemovePermissions(IEnumerable<string> permIds = null)
        {
            if (permIds == null)
            {
                _permissions.Clear();
            }
            else
            {
                _permissions.RemoveAll(itm => permIds.Contains(itm.PermissionId));
            }
        }
    }
}
