using System.ComponentModel.DataAnnotations;

namespace Charlie.OpenIam.Core
{
    /// <summary>
    /// 内置的权限
    /// </summary>
    public class BuiltInPermissions
    {
        private const string _prefix = "iam.";

        /// <summary>
        /// 创建用户
        /// </summary>
        [Display(Name = "创建用户")]
        public const string USER_CREATE = _prefix + "users.create";

        /// <summary>
        /// 更新用户
        /// </summary>
        [Display(Name = "更新用户")]
        public const string USER_UPDATE = _prefix + "users.update";

        /// <summary>
        /// 获取用户
        /// </summary>
        [Display(Name = "获取用户")]
        public const string USER_GET = _prefix + "users.get";

        /// <summary>
        /// 分配权限给用户
        /// </summary>
        [Display(Name = "分配权限给用户")]
        public const string USER_ASSIGN_PERM = _prefix + "users.assignperm";

        /// <summary>
        /// 分配角色给用户
        /// </summary>
        [Display(Name = "分配角色给用户")]
        public const string USER_ASSIGN_ROLE = _prefix + "users.assignrole";

        /// <summary>
        /// 激活、冻结用户
        /// </summary>
        [Display(Name = "激活、冻结用户")]
        public const string USER_ACTIVE = _prefix + "users.active";

        /// <summary>
        /// 删除用户
        /// </summary>
        [Display(Name = "删除用户")]
        public const string USER_DELETE = _prefix + "users.delete";

        /// <summary>
        /// 重置用户密码
        /// </summary>
        [Display(Name = "重置用户密码")]
        public const string USER_PWD_RESET = _prefix + "users.resetpwd";

        /// <summary>
        /// 更新权限
        /// </summary>
        [Display(Name = "更新权限")]
        public const string PERM_UPDATE = _prefix + "perm.update";

        /// <summary>
        /// 删除权限
        /// </summary>
        [Display(Name = "删除权限")]
        public const string PERM_DELETE = _prefix + "perm.delete";

        /// <summary>
        /// 获取所有权限
        /// </summary>
        [Display(Name = "获取所有权限")]
        public const string PERM_GETALL = _prefix + "perm.getall";

        /// <summary>
        /// 创建权限
        /// </summary>
        [Display(Name = "创建权限")]
        public const string PERM_CREATE = _prefix + "perm.create";

        /// <summary>
        /// 同步子系统的权限
        /// </summary>
        [Display(Name = "同步子系统的权限")]
        public const string PERM_SYNC = _prefix + "perm.sync";

        /// <summary>
        /// 新建角色
        /// </summary>
        [Display(Name = "新建角色")]
        public const string ROLE_CREATE = _prefix + "role.create";

        /// <summary>
        /// 更新角色
        /// </summary>
        [Display(Name = "更新角色")]
        public const string ROLE_UPDATE = _prefix + "role.update";

        /// <summary>
        /// 删除角色
        /// </summary>
        [Display(Name = "删除角色")]
        public const string ROLE_DELETE = _prefix + "role.delete";

        /// <summary>
        /// 更新角色中的权限
        /// </summary>
        [Display(Name = "更新角色中的权限")]
        public const string ROLE_PERM_UPDATE = _prefix + "role.perm.update";

        /// <summary>
        /// 获取角色及权限
        /// </summary>
        [Display(Name = "获取角色及权限")]
        public const string ROLE_GET = _prefix + "role.get";

        /// <summary>
        /// 获取组织
        /// </summary>
        [Display(Name = "获取组织")]
        public const string ORGS_GET = _prefix + "orgs.get";

        /// <summary>
        /// 创建组织
        /// </summary>
        [Display(Name = "创建组织")]
        public const string ORGS_CREATE = _prefix + "orgs.create";

        /// <summary>
        /// 更新组织
        /// </summary>
        [Display(Name = "更新组织")]
        public const string ORGS_UPDATE = _prefix + "orgs.update";

        /// <summary>
        /// 删除组织
        /// </summary>
        [Display(Name = "删除组织")]
        public const string ORGS_DELETE = _prefix + "orgs.delete";

        /// <summary>
        /// 新增组织中的默认角色
        /// </summary>
        [Display(Name = "新增组织中的默认角色")]
        public const string ORGS_ROLE_CREATE = _prefix + "orgs.role.create";

        /// <summary>
        /// 更新组织中的默认角色
        /// </summary>
        [Display(Name = "更新组织中的默认角色")]
        public const string ORGS_ROLE_UPDATE = _prefix + "orgs.role.update";

        /// <summary>
        /// 删除组织中的默认角色
        /// </summary>
        [Display(Name = "删除组织中的默认角色")]
        public const string ORGS_ROLE_DELETE = _prefix + "orgs.role.delete";

        /// <summary>
        /// 获取组织中的用户
        /// </summary>
        [Display(Name = "获取组织中的用户")]
        public const string ORGS_USER_GET = _prefix + "orgs.user.get";

        /// <summary>
        /// 移除组织中的用户
        /// </summary>
        [Display(Name = "移除组织中的用户")]
        public const string ORGS_USER_REMOVE= _prefix + "orgs.user.remove";

        /// <summary>
        /// 组织中增加用户
        /// </summary>
        [Display(Name = "组织中增加用户")]
        public const string ORGS_USER_ADD = _prefix + "orgs.user.add";

        /// <summary>
        /// 更新组织中的用户
        /// </summary>
        [Display(Name = "更新组织中的用户")]
        public const string ORGS_USER_UPDATE = _prefix + "orgs.user.update";

        /// <summary>
        /// 获取 Client
        /// </summary>
        [Display(Name = "获取 Client")]
        public const string CLIENT_GET = _prefix + "client.get";

        /// <summary>
        /// 创建 Client
        /// </summary>
        [Display(Name = "创建 Client")]
        public const string CLIENT_CREATE = _prefix + "client.create";

        /// <summary>
        /// 更新 Client
        /// </summary>
        [Display(Name = "更新 Client")]
        public const string CLIENT_UPDATE = _prefix + "client.update";

        /// <summary>
        /// 删除 Client
        /// </summary>
        [Display(Name = "删除 Client")]
        public const string CLIENT_DELETE = _prefix + "client.delete";

        /// <summary>
        /// 获取系统信息
        /// </summary>
        [Display(Name = "获取系统信息")]
        public const string SYS_GET = _prefix + "sys.get";

        /// <summary>
        /// 更新系统信息
        /// </summary>
        [Display(Name = "更新系统信息")]
        public const string SYS_UPDATE = _prefix + "sys.update";
    }
}
