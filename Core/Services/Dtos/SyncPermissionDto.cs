using System.Collections.Generic;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 同步权限 模型
    /// </summary>
    public class SyncPermissionDto
    {
        /// <summary>
        /// Client id，除了内置权限，其他添加的权限都需要有归属的 client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 权限集合
        /// </summary>
        public IEnumerable<PermissionNewDto> Permissions { get; set; }
    }
}
