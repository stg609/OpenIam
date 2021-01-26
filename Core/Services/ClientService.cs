using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Common.Helpers;
using Charlie.OpenIam.Core.Models.Repositories;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using IdentityServer4;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;

namespace Charlie.OpenIam.Core.Models.Services
{
    public class ClientService : IClientService
    {
        private readonly IMapper _mapper;
        private readonly IClientRepo _clientRepo;

        public ClientService(IMapper mapper, IClientRepo clientRepo)
        {
            _mapper = mapper;
            _clientRepo = clientRepo;
        }

        public async Task<PaginatedDto<ClientDto>> GetAllAsync(string name, IEnumerable<string> clientIds = null, IEnumerable<string> allowedClientIds = null, int pageSize = 10, int pageIndex = 1)
        {
            var results = await _clientRepo.GetAllAsync(name, clientIds, allowedClientIds, pageSize, pageIndex);
            return _mapper.Map<PaginatedDto<ClientDto>>(results);
        }

        public async Task<ClientDto> GetAsync(string clientId, IEnumerable<string> allowedClientIds = null)
        {
            if (allowedClientIds != null && !allowedClientIds.Contains(clientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            var client = await _clientRepo.GetAsync(clientId);
            return client == null ? null : _mapper.Map<ClientDto>(client);
        }

        public async Task<(string ClientId, string PlainSecret)> CreateAsync(ClientNewDto model)
        {
            if (await _clientRepo.IsExistedAsync(model.ClientUri))
            {
                throw new IamException(HttpStatusCode.BadRequest, "该客户端地址已经存在！");
            }

            ICollection<string> allowedCorsOrigins = null;
            if (model.AllowedCorsOrigins != null)
            {
                allowedCorsOrigins = model.AllowedCorsOrigins.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var itm in allowedCorsOrigins)
                {
                    if (!itm.IsUrl())
                    {
                        throw new IamException(HttpStatusCode.BadRequest, $"{itm} 并不是合法的允许的跨域地址,必须是 Url 形式");
                    }
                }
            }

            model.ClientName = model.ClientName.Trim();
            var allowedScopes = model.AllowedScopes?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(sp => sp.Trim()).ToList() ?? new List<string>();

            // 排除 id scopes 以及 已经存在的 api scopes
            var newScopes = allowedScopes.Except(await _clientRepo.GetIdentityResourceNamesAsync(allowedScopes))
                .Except(await _clientRepo.GetApiResourceNamesAsync(allowedScopes));

            _clientRepo.AddApiResources(newScopes);

            // 增加 IAM 需要的 scopes
            if (!allowedScopes.Contains("iam"))
            {
                allowedScopes.Add("iam");
            }

            if (!allowedScopes.Contains("iamApi"))
            {
                allowedScopes.Add("iamApi");
            }

            if (!allowedScopes.Contains(IdentityServerConstants.StandardScopes.OpenId))
            {
                allowedScopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            }

            if (!allowedScopes.Contains(IdentityServerConstants.StandardScopes.Profile))
            {
                allowedScopes.Add(IdentityServerConstants.StandardScopes.Profile);
            }

            if (!allowedScopes.Contains(IdentityServerConstants.StandardScopes.OfflineAccess))
            {
                allowedScopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
            }

            string clientId = Guid.NewGuid().ToString();
            string secret = Helper.GetRandomString(30);
            string[] redirectUris = null;
            string[] postLogoutRedirectUris = null;
            if (String.IsNullOrWhiteSpace(model.RedirectUris))
            {
                // signin-oidc 与 静默更新都是要作为回调的一部分
                redirectUris = new[] { $"{model.ClientUri.TrimEnd('/')}/signin-oidc", $"{model.ClientUri.TrimEnd('/')}/silent-renew" };
            }
            else
            {
                redirectUris = model.RedirectUris.Split(",", StringSplitOptions.RemoveEmptyEntries);
            }

            if (String.IsNullOrWhiteSpace(model.PostLogoutRedirectUris))
            {
                postLogoutRedirectUris = new[] { $"{model.ClientUri.TrimEnd('/')}/signout-callback-oidc" };
            }
            else
            {
                postLogoutRedirectUris = model.PostLogoutRedirectUris.Split(",", StringSplitOptions.RemoveEmptyEntries);
            }

            IdentityServer4.Models.Client client = new IdentityServer4.Models.Client
            {
                ClientId = clientId,
                ClientSecrets = { new Secret(secret.Sha256()) },
                AlwaysSendClientClaims = model.AlwaysSendClientClaims,
                AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken,
                AccessTokenLifetime = model.AccessTokenLifetime,
                AllowedScopes = allowedScopes,
                AllowedCorsOrigins = allowedCorsOrigins,
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                AllowOfflineAccess = true, // 允许使用 refresh token 来刷新（因为 Cookie 一般时间会大于 token 的有效期）
                ClientUri = model.ClientUri,
                ClientName = model.ClientName,
                Description = model.Description,
                Enabled = true,
                IdentityTokenLifetime = model.IdentityTokenLifetime,
                LogoUri = model.LogoUri,
                RedirectUris = redirectUris,
                PostLogoutRedirectUris = postLogoutRedirectUris,
                RequireConsent = false,
                RequireClientSecret = false,
                ClientClaimsPrefix = Constants.CLIENT_CLAIM_PREFIX,

                // 对于 Code 模式采用 Pkce 扩展
                RequirePkce = true,

                Claims = new ClientClaim[]
                {
                    // 每个 Client 都允许进行同步 perm 的操作
                    new ClientClaim(BuiltInPermissions.PERM_SYNC,BuiltInPermissions.PERM_SYNC)
                }
            };

            _clientRepo.Add(client);

            //await _clientDbContext.SaveChangesAsync();
            return (clientId, secret);
        }

        public async Task UpdateAsync(string clientId, ClientUpdateDto model, IEnumerable<string> allowedClientIds = null)
        {
            ICollection<string> allowedCorsOrigins = null;
            if (model.AllowedCorsOrigins != null)
            {
                allowedCorsOrigins = model.AllowedCorsOrigins.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var itm in allowedCorsOrigins)
                {
                    if (!itm.IsUrl())
                    {
                        throw new IamException(HttpStatusCode.BadRequest, $"{itm} 并不是合法的允许的跨域地址,必须是 Url 形式");
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(model.ClientUri) && !model.ClientUri.IsUrl())
            {
                throw new IamException(HttpStatusCode.BadRequest, $"{model.ClientUri} 并不是合法的客户端地址,必须是 Url 形式");
            }

            if (allowedClientIds != null && !allowedClientIds.Contains(clientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            var client = await _clientRepo.GetRawClientAsync(clientId, false);
            if (client == null)
            {
                throw new IamException(HttpStatusCode.BadRequest, "Client 不存在");
            }

            if (!String.IsNullOrWhiteSpace(model.ClientUri) && client.ClientUri != model.ClientUri)
            {
                if (await _clientRepo.IsExistedAsync(model.ClientUri))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "该客户端地址已经存在！");
                }
            }

            List<string> allowedScopes = null;
            if (model.AllowedScopes != null && !String.IsNullOrWhiteSpace(model.AllowedScopes))
            {
                allowedScopes = model.AllowedScopes.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(sp => sp.Trim()).ToList();

                // 排除 id scopes 以及 已经存在的 api scopes
                var newScopes = allowedScopes.Except(await _clientRepo.GetIdentityResourceNamesAsync(allowedScopes))
                    .Except(await _clientRepo.GetApiResourceNamesAsync(allowedScopes));

                _clientRepo.AddApiResources(newScopes);

                // 增加 IAM 需要的 scopes
                if (!allowedScopes.Contains("iam"))
                {
                    allowedScopes.Add("iam");
                }

                if (!allowedScopes.Contains("iamApi"))
                {
                    allowedScopes.Add("iamApi");
                }

                if (!allowedScopes.Contains("openid"))
                {
                    allowedScopes.Add("openid");
                }

                if (!allowedScopes.Contains("profile"))
                {
                    allowedScopes.Add("profile");
                }

                if (!allowedScopes.Contains(IdentityServerConstants.StandardScopes.OfflineAccess))
                {
                    allowedScopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
                }
            }

            string[] redirectUris = null;
            string[] postLogoutRedirectUris = null;
            if (model.RedirectUris != null && String.IsNullOrWhiteSpace(model.RedirectUris))
            {
                redirectUris = new[] { $"{model.ClientUri.TrimEnd('/')}/signin-oidc" };
            }
            else
            {
                redirectUris = model.RedirectUris?.Split(",", StringSplitOptions.RemoveEmptyEntries);
            }

            if (model.PostLogoutRedirectUris != null && String.IsNullOrWhiteSpace(model.PostLogoutRedirectUris))
            {
                postLogoutRedirectUris = new[] { $"{model.ClientUri.TrimEnd('/')}/signout-callback-oidc" };
            }
            else
            {
                postLogoutRedirectUris = model.PostLogoutRedirectUris?.Split(",", StringSplitOptions.RemoveEmptyEntries);
            }

            var clientModel = client.ToModel();
            clientModel.ClientName = model.ClientName ?? clientModel.ClientName;
            clientModel.AccessTokenLifetime = model.AccessTokenLifetime ?? clientModel.AccessTokenLifetime;
            clientModel.AllowedCorsOrigins = model.AllowedCorsOrigins == null ? clientModel.AllowedCorsOrigins : (String.IsNullOrWhiteSpace(model.AllowedCorsOrigins) ? null : model.AllowedCorsOrigins.Split(","));
            clientModel.AlwaysSendClientClaims = model.AlwaysSendClientClaims ?? clientModel.AlwaysSendClientClaims;
            clientModel.AllowedScopes = allowedScopes == null ? clientModel.AllowedScopes : allowedScopes;

            clientModel.Description = model.Description ?? clientModel.Description;
            clientModel.Enabled = model.IsEnabled;
            clientModel.IdentityTokenLifetime = model.IdentityTokenLifetime ?? clientModel.IdentityTokenLifetime;
            clientModel.LogoUri = model.LogoUri ?? (String.IsNullOrWhiteSpace(clientModel.LogoUri) ? null : clientModel.LogoUri);
            clientModel.ClientUri = model.ClientUri ?? (String.IsNullOrWhiteSpace(clientModel.ClientUri) ? null : clientModel.ClientUri);
            clientModel.RedirectUris = redirectUris == null ? clientModel.RedirectUris : redirectUris;
            clientModel.PostLogoutRedirectUris = postLogoutRedirectUris == null ? clientModel.PostLogoutRedirectUris : postLogoutRedirectUris;

            var updatedClient = clientModel.ToEntity();

            client.ClientName = updatedClient.ClientName;
            client.AccessTokenLifetime = updatedClient.AccessTokenLifetime;
            client.AllowedCorsOrigins?.Clear();
            client.AllowedCorsOrigins = updatedClient.AllowedCorsOrigins;

            client.AlwaysSendClientClaims = updatedClient.AlwaysSendClientClaims;

            client.AllowedScopes?.Clear();
            client.AllowedScopes = updatedClient.AllowedScopes;

            client.Description = updatedClient.Description;
            client.Enabled = updatedClient.Enabled;
            client.IdentityTokenLifetime = updatedClient.IdentityTokenLifetime;
            client.LogoUri = updatedClient.LogoUri;
            client.ClientUri = updatedClient.ClientUri;
            client.RedirectUris?.Clear();
            client.RedirectUris = updatedClient.RedirectUris;
            client.PostLogoutRedirectUris = updatedClient.PostLogoutRedirectUris;
        }

        public async Task SwitchAsync(string clientId, bool isEnabled, IEnumerable<string> allowedClientIds = null)
        {
            if (allowedClientIds != null && !allowedClientIds.Contains(clientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            var client = await _clientRepo.GetRawClientAsync(clientId, false);
            client.Enabled = isEnabled;
        }

        public async Task<string> ResetSecretAsync(string clientId, IEnumerable<string> allowedClientIds = null)
        {
            if (allowedClientIds != null && !allowedClientIds.Contains(clientId))
            {
                throw new IamException(HttpStatusCode.BadRequest, "无权操作");
            }

            var client = await _clientRepo.GetRawClientAsync(clientId, false);

            string secret = Helper.GetRandomString(30);
            var clientModel = client.ToModel();
            clientModel.ClientSecrets = new[] { new Secret(secret.Sha256()) };

            client.ClientSecrets = clientModel.ToEntity().ClientSecrets;

            return secret;
        }

        public async Task RemoveAsync(IEnumerable<string> targetClientIds, IEnumerable<string> allowedClientIds = null)
        {
            if (allowedClientIds != null)
            {
                var notAllowedClientIds = allowedClientIds.Except(targetClientIds);
                if (notAllowedClientIds.Any())
                {
                    throw new IamException(HttpStatusCode.BadRequest, "无权操作");
                }
            }

            await _clientRepo.RemoveAsync(targetClientIds);
        }
    }
}
