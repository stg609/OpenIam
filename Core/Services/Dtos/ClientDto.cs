using System.Collections.Generic;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class ClientDto
    {
        public bool AlwaysSendClientClaims { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public int AccessTokenLifetime { get; set; }
        public IEnumerable<string> AllowedScopes { get; set; }
        public IEnumerable<string> AllowedCorsOrigins { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }

        public string PlainSecret { get; set; }

        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public string LogoUri { get; set; }

        /// <summary>
        /// 客户端地址
        /// </summary>
        public string ClientUri { get; set; }

        public IEnumerable<string> RedirectUris { get; set; }
        public IEnumerable<string> PostLogoutRedirectUris { get; set; }
        public int Id { get; set; }
    }
}
