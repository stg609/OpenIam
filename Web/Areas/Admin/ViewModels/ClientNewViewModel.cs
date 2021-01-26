using System.ComponentModel.DataAnnotations;

namespace Charlie.OpenIam.Web.Areas.Admin.ViewModels
{
    public class ClientNewViewModel
    {
        public bool AlwaysSendClientClaims { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public int AccessTokenLifetime { get; set; } = 3600;

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
        [Required]
        public string ClientName { get; set; }
        public string Description { get; set; }
        public int IdentityTokenLifetime { get; set; } = 3600;

        public string LogoUri { get; set; }

        /// <summary>
        /// 客户端根地址，即：http://localhost:80
        /// </summary>
        /// <remarks>后续判断权限的时候，可用于比对当前访问目标</remarks>
        [Required, Url]
        public string ClientUri { get; set; }

        /// <summary>
        /// 回调地址，多个地址用英文逗号分隔。这些地址主要包括用于完成 OAuth 流程的 signin-oidc 以及 用于 SPA 静默更新的 silent-renew
        /// </summary>
        public string RedirectUris { get; set; }

        /// <summary>
        /// 登出地址，多个地址用英文逗号分隔
        /// </summary>
        public string PostLogoutRedirectUris { get; set; }

    }
}
