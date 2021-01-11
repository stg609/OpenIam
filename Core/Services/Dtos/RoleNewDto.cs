namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 新增角色 Dto
    /// </summary>
    public class RoleNewDto
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属的 ClientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 是否是当前客户端的管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
