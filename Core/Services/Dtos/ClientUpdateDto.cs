namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class ClientUpdateDto
    {
        public bool? AlwaysSendClientClaims { get; set; }
        public bool? AlwaysIncludeUserClaimsInIdToken { get; set; }
        public int? AccessTokenLifetime { get; set; }

        /// <summary>
        /// 允许的 Scope，多个 Scope 用英文逗号分隔
        /// </summary>
        public string AllowedScopes { get; set; }

        /// <summary>
        /// 允许的跨域地址，多个地址用英文逗号分隔
        /// </summary>
        public string AllowedCorsOrigins { get; set; }

        /// <summary>
        /// 必须唯一
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Id Token 生命周期
        /// </summary>
        public int? IdentityTokenLifetime { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string LogoUri { get; set; }

        /// <summary>
        /// 客户端地址
        /// </summary>
        public string ClientUri { get; set; }

        /// <summary>
        /// 回调地址，多个地址用英文逗号分隔
        /// </summary>
        public string RedirectUris { get; set; }

        /// <summary>
        /// 登出地址，多个地址用英文逗号分隔
        /// </summary>
        public string PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
