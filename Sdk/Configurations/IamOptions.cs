using System.ComponentModel.DataAnnotations;

namespace Charlie.OpenIam.Sdk.Configurations
{
    /// <summary>
    /// Iam 配置
    /// </summary>
    public class IamOptions
        :IamBasicOptions
    {
        /// <summary>
        /// Client Id
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Client 密钥
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }

        /// <summary>
        /// 是否要求 Https
        /// </summary>
        public bool RequireHttpsMetadata { get; set; }

        ///// <summary>
        ///// 是否保存 Token 到 Cookie 中，为了减小 Cookie 的体积，该属性默认为 false 
        ///// </summary>
        //public bool SaveTokens { get; set; }

        /// <summary>
        /// 是否要从 UserInfoEndpoint 获取额外的用户 Claim 用于构建 HttpContext.User 中的 Claims
        /// </summary>
        /// <remarks>
        /// 如果需要在登陆后的 User.Claims 中获取工号，则此项需设置为 true
        /// </remarks>
        public bool GetClaimsFromUserInfoEndpoint { get; set; }

        /// <summary>
        /// 要请求的 Scopes
        /// </summary>
        public string[] Scopes { get; set; }

        /// <summary>
        /// 当 403 的时候跳转到 Client 端的地址
        /// </summary>
        public string AccessDeniedPath { get; set; }
    }
}
