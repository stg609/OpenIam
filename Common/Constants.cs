namespace Charlie.OpenIam.Common
{
    /// <summary>
    /// 常量
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// 超级管理员的 claim，如果包含该 claim，则说明当前用户是 超级管理员
        /// </summary>
        public static readonly string SUPERADMIN_CLAIM_TYPE = "super";

        /// <summary>
        /// 工号 Claim
        /// </summary>
        public static readonly string CLAIM_TYPES_SALER = "jobno";

        /// <summary>
        /// 用于在 access token 中包含 jobno 这个claim
        /// </summary>
        public static readonly string IAM_API_SCOPE = "iamApi";

        /// <summary>
        /// 用于在 id token 中包含 jobno 这个 claim
        /// </summary>
        public static readonly string IAM_ID_SCOPE = "iam";

        /// <summary>
        /// 冒号分隔符
        /// </summary>
        public static readonly string ColonDelimiter = ":";

        /// <summary>
        /// 客户端自己的 Claim 前缀
        /// </summary>
        public static readonly string CLIENT_CLAIM_PREFIX = "client_";

        /// <summary>
        /// 游客 角色
        /// </summary>
        public static readonly string ROLES_GUEST = "Guest";

        /// <summary>
        /// Admin 角色
        /// </summary>
        public static readonly string ROLES_ADMIN = "Admin";
    }
}
