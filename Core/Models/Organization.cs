using System;
using System.Collections.Generic;

namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 组织机构信息
    /// </summary>
    public class Organization : IAuditable, ISoftDeletable
    {
        /// <summary>
        /// 组织机构id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 组织机构名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Desc { get; private set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; private set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// 上级
        /// </summary>
        public Organization Parent { get; private set; }
        private string _parentId;

        /// <summary>
        /// 用户组织
        /// </summary>
        public IReadOnlyCollection<UserOrganization> UserOrganizations => _userOrganizations;
        private readonly List<UserOrganization> _userOrganizations = new List<UserOrganization>();

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

        protected Organization()
        {

        }

        public Organization(string id, string name, string desc, string address, string mobile,  string parentId, bool isEnabled = true)
        {
            Id = id;
            Name = name;
            Address = address;
            Desc = desc;
            Mobile = mobile;
            Enabled = isEnabled;
            _parentId = String.IsNullOrWhiteSpace(parentId) ? null : parentId.Trim();
        }

        public void Update(string name, string desc, string address, string mobile,  string parentId, bool? isEnabled)
        {
            Name = name ?? Name;
            Address = address ?? Address;
            Desc = desc ?? Desc;
            Mobile = mobile ?? Mobile;
            Enabled = isEnabled.HasValue ? isEnabled.Value : Enabled;
            _parentId = parentId == null ? _parentId : (parentId == String.Empty ? null : parentId);
        }

        public void AddUser(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                return;
            }
            _userOrganizations.Add(new UserOrganization(Id, userId, true));
        }

        public void AddRole(string roleId)
        {
            if (String.IsNullOrWhiteSpace(roleId))
            {
                return;
            }
            _organizationRoles.Add(new OrganizationRole(Id, roleId));
        }

        public void RemoveDefaultRoles(Predicate<OrganizationRole> match = null)
        {
            if (match == null)
            {
                _organizationRoles.Clear();
            }
            else
            {
                _organizationRoles.RemoveAll(match);
            }
        }

        public void RemoveUsers(Predicate<UserOrganization> match = null)
        {
            if (match == null)
            {
                _userOrganizations.Clear();
            }
            else
            {
                _userOrganizations.RemoveAll(match);
            }
        }

    }
}
