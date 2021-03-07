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
        /// 名
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string Saler { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 座右铭
        /// </summary>
        public string Motto { get; set; }
    }
}
