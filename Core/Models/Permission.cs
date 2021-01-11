using System;
using System.Collections.Generic;
using Charlie.OpenIam.Abstraction.Dtos;

namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Permission : IAuditable, ISoftDeletable
    {
        /// <summary>
        /// 编号，全局唯一
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 权限的 Key（同一Client中必须唯一）
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 人可读的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Client 的编号
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        /// 类型： 0 菜单 2 Api 
        /// </summary>
        public PermissionType Type { get; private set; }

        /// <summary>
        /// 父级权限
        /// </summary>
        public Permission Parent { get; private set; }
        private string _parentId;

        #region 用于 Type 为 View 的时候的额外字段

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; private set; }

        #endregion

        /// <summary>
        /// 用户权限
        /// </summary>
        public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions;
        private readonly List<UserPermission> _userPermissions = new List<UserPermission>();

        /// <summary>
        /// 角色权限
        /// </summary>
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;
        private readonly List<RolePermission> _rolePermissions = new List<RolePermission>();

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

        protected Permission() { }

        public Permission(string id, string clientId, PermissionType type, string key, string name, string desc = null, string parentId = null, string url = null, string icon = null, int? order = null, int? level = null)
        {
            ClientId = clientId;
            Id = id;
            Key = key;
            Name = name;
            Type = type;
            Description = desc;
            _parentId = String.IsNullOrWhiteSpace(parentId) ? null : parentId.Trim();

            if (type == PermissionType.View)
            {
                Url = url;
                Icon = icon;
                Level = level.HasValue ? level.Value : 0;
                Order = order.HasValue ? order.Value : 0;
            }

            Enabled = true;
        }

        public void Update(string name, string description, PermissionType? type, string parentId = null, string url = null, string icon = null, int? order = null, int? level = null)
        {
            Name = name ?? Name;
            Description = description ?? Description;
            Type = type ?? Type;

            if (parentId != null)
            {
                if (String.IsNullOrEmpty(parentId))
                {
                    // 如果是空字符串，则表示是要去掉这个 _parentId
                    _parentId = null;
                }
                else
                {
                    _parentId = parentId;
                }
            }

            if (Type == PermissionType.View)
            {
                Url = url ?? Url;
                Icon = icon ?? Icon;
                Level = level.HasValue ? level.Value : Level;
                Order = order.HasValue ? order.Value : Order;
            }
        }
    }
}
