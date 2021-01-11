namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 更新角色的 DTO  
    /// </summary>
    public class RoleUpdateDto
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 是否是当前客户端的管理员
        /// </summary>
        public bool? IsAdmin { get; set; }
    }
}
