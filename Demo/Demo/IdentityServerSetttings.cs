using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class IdentityServerSettings
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public bool GetClaimsFromUserInfoEndpoint { get; set; }

        public string[] Scopes { get; set; }

        public string AccessDeniedPath { get; set; }
    }
}
