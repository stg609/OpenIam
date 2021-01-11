namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class ClientNewDto
    {
        public bool AlwaysSendClientClaims { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public int AccessTokenLifetime { get; set; } = 3600;
        public int IdentityTokenLifetime { get; set; } = 3600;

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

        public string Description { get; set; }


        public string LogoUri { get; set; }

        /// <summary>
        /// 客户端根地址，即：http://localhost:80
        /// </summary>
        /// <remarks>后续判断权限的时候，可用于比对当前访问目标</remarks>
        public string ClientUri { get; set; }

        /// <summary>
        /// 回调地址，多个地址用英文逗号分隔
        /// </summary>
        public string RedirectUris { get; set; }

        /// <summary>
        /// 登出地址，多个地址用英文逗号分隔
        /// </summary>
        public string PostLogoutRedirectUris { get; set; }

    }
}
