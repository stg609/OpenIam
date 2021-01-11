using System.Collections.Generic;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 组织 管理员视图模型
    /// </summary>
    public class OrganizationDto
        : OrganizationUpdateDto, IHasParentIdAndChildren<OrganizationDto>
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();

        /// <summary>
        /// 子级
        /// </summary>
        public List<OrganizationDto> Children { get; set; } = new List<OrganizationDto>();

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; }
    }
}
