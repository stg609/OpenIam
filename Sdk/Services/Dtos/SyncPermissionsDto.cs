using System.Collections.Generic;
using Charlie.OpenIam.Abstraction.Dtos;

namespace Charlie.OpenIam.Sdk.Services.Dtos
{
    /// <summary>
    /// 同步权限模型
    /// </summary>
    public class SyncPermissionsDto
    {
        /// <summary>
        /// Client id，除了内置权限，其他添加的权限都需要有归属的 client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 权限集合
        /// </summary>
        public List<PermissionDto> Permissions { get; set; }
    }
}
