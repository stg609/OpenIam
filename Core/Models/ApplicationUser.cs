using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Charlie.OpenIam.Core.Models
{

    /// <summary>
    /// 用户实体
    /// </summary>
    public class ApplicationUser : IdentityUser, IAuditable, ISoftDeletable
    {
        /// <summary>
        /// 内部系统的人员编号，可以是工号，必须能唯一关联用户
        /// </summary>
        public string JobNo { get; private set; }

        /// <summary>
        /// 名
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; private set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; private set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard { get; private set; }

        /// <summary>
        /// 身份证正面照片
        /// </summary>
        public string IdCardFaceImg { get; private set; }

        /// <summary>
        /// 身份证背面地址
        /// </summary>
        public string IdCardBackImg { get; private set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string HomeAddress { get; private set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 个人封面
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 座右铭
        /// </summary>
        public string Motto { get; set; }

        /// <summary>
        /// Github 地址
        /// </summary>
        public string Github { get; set; }

        /// <summary>
        /// 推特账号
        /// </summary>
        public string Twitter { get; set; }

        /// <summary>
        /// 新浪微博
        /// </summary>
        public string SinaWeibo { get; set; }

        /// <summary>
        /// 个人备注，支持 HTML
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 用户组织
        /// </summary>
        public IReadOnlyCollection<UserOrganization> UserOrganizations => _userOrganizations;
        private readonly List<UserOrganization> _userOrganizations = new List<UserOrganization>();

        /// <summary>
        /// 用户权限
        /// </summary>
        public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions;
        private readonly List<UserPermission> _userPermissions = new List<UserPermission>();

        /// <summary>
        /// 最后一次登陆的 IP
        /// </summary>
        public string LastIp { get; private set; }

        /// <summary>
        /// 最后一次登陆时间
        /// </summary>
        public DateTime LastLoginAt { get; private set; }

        /// <summary>
        /// 是否已经激活，未激活的用户将无法登陆及使用功能
        /// </summary>
        public bool IsActive { get; private set; }

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

        protected ApplicationUser()
        {

        }

        public ApplicationUser(string userName, string jobNo = null, string email = null, string homeAddress = null, string idCard = null, string phone = null, string firstName = null, string lastName = null, string position = null, Gender gender = Gender.Unknown, bool isActive = false, string motto = null, string avatar = null, string cover = null, string github = null, string twitter = null, string sinaWeibo = null, string note = null) : base(userName)
        {
            JobNo = jobNo;
            Email = email;
            HomeAddress = homeAddress;
            IdCard = idCard;
            PhoneNumber = phone;
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            Gender = gender;
            IsActive = isActive;
            Avatar = avatar;
            Cover = cover;
            Motto = motto;
            Github = github;
            Twitter = twitter;
            SinaWeibo = sinaWeibo;
            Note = note;
        }

        public void AddOrganizations(string orgId, bool isCharger = false)
        {
            _userOrganizations.Add(new UserOrganization(orgId, Id, isCharger));
        }

        public void Update(string jobNo, string email, string homeAddress, string idCard, string phone, string firstName, string lastName, string position, Gender? gender, bool? isActive, string motto, string avatar, string cover, string github, string twitter, string sinaWeibo, string note)
        {
            JobNo = jobNo ?? JobNo;
            Email = email ?? Email;
            HomeAddress = homeAddress ?? HomeAddress;
            IdCard = idCard ?? IdCard;
            PhoneNumber = phone ?? PhoneNumber;
            FirstName = firstName ?? FirstName;
            LastName = lastName ?? LastName;
            Position = position ?? Position;
            Gender = gender ?? Gender;
            IsActive = isActive ?? IsActive;
            Avatar = avatar ?? Avatar;
            Cover = cover ?? Cover;
            Motto = motto ?? Motto;
            Github = github ?? Github;
            Twitter = twitter ?? Twitter;
            SinaWeibo = sinaWeibo ?? SinaWeibo;
            Note = note ?? Note;
        }

        public void RemoveOrganizations()
        {
            _userOrganizations.Clear();
        }

        public void Switch(bool? isActive = null)
        {
            if (isActive.HasValue)
            {
                IsActive = isActive.Value;
            }
            else
            {
                IsActive = !IsActive;
            }
        }

        public void RemovePermission(UserPermission existed)
        {
            _userPermissions.Remove(existed);
        }

        public void AddPermission(string permissionId, PermissionAction action, string[] permissionRoleIds = null)
        {
            _userPermissions.Add(new UserPermission(Id, permissionId, action, permissionRoleIds));
        }
    }
}
