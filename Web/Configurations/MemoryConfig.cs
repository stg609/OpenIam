using System.Collections.Generic;
using Charlie.OpenIam.Common;
using IdentityServer4.Models;

namespace Charlie.OpenIam.Web.Configurations
{
    public class MemoryConfig
    {
        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResource(Constants.IAM_ID_SCOPE, new[]
                {
                    Constants.CLAIM_TYPES_SALER
                })
            };
        }

        public static List<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(Constants.IAM_API_SCOPE, new[]
                {
                    Constants.CLAIM_TYPES_SALER
                })                
            };
        }

        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(Constants.IAM_API_SCOPE, new[]
                {
                    Constants.CLAIM_TYPES_SALER
                })
                {
                    Scopes = { Constants.IAM_API_SCOPE }
                }
            };
        }
    }
}
