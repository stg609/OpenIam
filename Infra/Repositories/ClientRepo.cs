using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Models.Repositories;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Charlie.OpenIam.Infra.Repositories
{
    public class ClientRepo : IClientRepo
    {
        private readonly IamConfigurationDbContext _clientDbContext;

        public ClientRepo(IamConfigurationDbContext clientDbContext)
        {
            _clientDbContext = clientDbContext;
        }

        public async Task<PaginatedDto<Client>> GetAllAsync(string name = null, IEnumerable<string> clientIds = null, IEnumerable<string> allowedClientIds = null, int pageSize = 10, int pageIndex = 1)
        {
            var query = _clientDbContext.Clients.AsNoTracking();

            if (allowedClientIds != null && allowedClientIds.Any())
            {
                query = query.Where(itm => allowedClientIds.Contains(itm.ClientId));
            }

            if (!String.IsNullOrWhiteSpace(name))
            {
                query = query.Where(itm => EF.Functions.ILike(itm.ClientName, $"%{name}%"));
            }

            if (clientIds != null && clientIds.Any())
            {
                query = query.Where(itm => clientIds.Contains(itm.ClientId));
            }

            IEnumerable<Client> results;
            if (pageSize > 0)
            {
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                results = (await query
                   .Include(itm => itm.AllowedScopes)
                   .Include(itm => itm.AllowedCorsOrigins)
                   .Include(itm => itm.RedirectUris)
                   .OrderByDescending(itm => itm.Created)
                   .Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize)
                   .ToListAsync())
                   .Select(itm => itm.ToModel());
            }
            else
            {
                results = (await query
                  .Include(itm => itm.AllowedScopes)
                  .Include(itm => itm.AllowedCorsOrigins)
                  .Include(itm => itm.RedirectUris)
                  .OrderByDescending(itm => itm.Created)
                  .ToListAsync())
                  .Select(itm => itm.ToModel());
            }
            int total = await query.CountAsync();

            return new PaginatedDto<Client>
            {
                Data = results,
                Total = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Client> GetAsync(string clientId)
        {
            var result = await GetRawClientAsync(clientId);
            return result?.ToModel();
        }

        public async Task<IdentityServer4.EntityFramework.Entities.Client> GetRawClientAsync(string clientId, bool isReadonly = true)
        {
            var query = isReadonly ? _clientDbContext.Clients.AsNoTracking() : _clientDbContext.Clients;
            var result = (await query
                .Include(itm => itm.AllowedScopes)
                .Include(itm => itm.AllowedCorsOrigins)
                .Include(itm => itm.RedirectUris)
                .Include(itm => itm.PostLogoutRedirectUris)
                .SingleOrDefaultAsync(itm => itm.ClientId == clientId));
            return result;
        }

        public async Task<bool> IsExistedAsync(string clientUri)
        {
            var query = _clientDbContext.Clients.Where(itm => EF.Functions.ILike(itm.ClientUri, $"{clientUri}"));
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<string>> GetApiResourceNamesAsync(IEnumerable<string> allowedScopes)
        {
            return await _clientDbContext.ApiResources.AsNoTracking()
                .Where(itm => allowedScopes.Contains(itm.Name))
                .Select(itm => itm.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetIdentityResourceNamesAsync(IEnumerable<string> allowedScopes)
        {
            return await _clientDbContext.IdentityResources.AsNoTracking()
                .Where(itm => allowedScopes.Contains(itm.Name))
                .Select(itm => itm.Name)
                .ToListAsync();
        }

        public void AddApiResources(IEnumerable<string> resources)
        {
            if (resources == null || !resources.Any())
            {
                return;
            }

            List<IdentityServer4.EntityFramework.Entities.ApiResource> apiResources = new List<IdentityServer4.EntityFramework.Entities.ApiResource>();
            List<IdentityServer4.EntityFramework.Entities.ApiScope> apiScopes = new List<IdentityServer4.EntityFramework.Entities.ApiScope>();

            foreach (var res in resources)
            {
                if (String.IsNullOrWhiteSpace(res))
                {
                    continue;
                }

                apiResources.Add((new ApiResource(res)
                {
                    Scopes = new[] { res }
                }).ToEntity());
                apiScopes.Add((new ApiScope(res)).ToEntity());
            }

            _clientDbContext.ApiResources.AddRange(apiResources);
            _clientDbContext.ApiScopes.AddRange(apiScopes);
        }

        public void Add(Client client)
        {
            _clientDbContext.Clients.Add(client.ToEntity());
        }

        public async Task RemoveAsync(IEnumerable<string> clientIds)
        {
            var clients = await _clientDbContext.Clients.Where(itm => clientIds.Contains(itm.ClientId)).ToListAsync();
            _clientDbContext.RemoveRange(clients);
        }
    }
}
