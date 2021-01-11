namespace Charlie.OpenIam.Sdk.Services.Dtos
{
    /// <summary>
    /// 用户基本信息
    /// </summary>
    public class UserBasicInfoDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string Saler { get; set; }
    }
}
