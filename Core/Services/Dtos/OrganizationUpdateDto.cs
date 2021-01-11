namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 更新组织 视图模型
    /// </summary>
    public class OrganizationUpdateDto
    {
        /// <summary>
        /// 组织机构名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }       

        /// <summary>
        /// 上级组织编号
        /// </summary>
        public string ParentId { get; set; }
    }
}
