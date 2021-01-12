using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Charlie.OpenIam.Infra.Repositories
{
    public class PermissionRepo : IPermissionRepo
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsExistedAsync(string id = null, string clientId = null, string key = null)
        {
            var query = _context.Permissions.AsNoTracking();
            if (!String.IsNullOrWhiteSpace(id))
            {
                query = query.Where(itm => itm.Id == id);
            }
            if (!String.IsNullOrWhiteSpace(clientId))
            {
                query = query.Where(itm => itm.ClientId == clientId);
            }
            if (!String.IsNullOrWhiteSpace(key))
            {
                query = query.Where(itm => itm.Key == key);
            }
            return await query.AnyAsync();
        }

        public async Task<Permission> GetAsync(string id = null, string key = null, string clientId = null, bool isReadonly = true)
        {
            if(String.IsNullOrWhiteSpace(id) && String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException();
            }

            var query = isReadonly ? _context.Permissions.AsNoTracking() :
                _context.Permissions;

            if(!String.IsNullOrWhiteSpace(id))
            {
                query = query.Where(itm => itm.Id == id);
            }

            if(!String.IsNullOrWhiteSpace(key))
            {
                query = query.Where(itm => itm.Key == key);
            }

            if(!String.IsNullOrWhiteSpace(clientId))
            {
                query = query.Where(itm => itm.ClientId == clientId);
            }

            return await query
                .Include(itm => itm.Parent)
                .SingleOrDefaultAsync();
        }

        public void Add(Permission permission)
        {
            _context.Permissions.Add(permission);
        }

        public async Task<IEnumerable<Permission>> GetAllAsync(string name = null, string key = null, string url = null, string targetClientId = null, PermissionType? type = null, IEnumerable<string> allowedClientIds = null)
        {
            var query = _context.Permissions.AsNoTracking();
            if (allowedClientIds != null && allowedClientIds.Any())
            {
                if (!String.IsNullOrWhiteSpace(targetClientId) && !allowedClientIds.Contains(targetClientId))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "无权操作");
                }

                query = query.Where(itm => allowedClientIds.Contains(itm.ClientId));
            }

            if (!String.IsNullOrWhiteSpace(targetClientId))
            {
                query = query.Where(itm => itm.ClientId == targetClientId);
            }

            if (!String.IsNullOrWhiteSpace(name))
            {
                query = query.Where(itm => itm.Name.Contains(name));
            }

            if (!String.IsNullOrWhiteSpace(key))
            {
                query = query.Where(itm => itm.Key.Contains(key));
            }

            if (!String.IsNullOrWhiteSpace(url))
            {
                query = query.Where(itm => itm.Url.Contains(url));
            }

            if (type.HasValue)
            {
                query = query.Where(itm => itm.Type == type.Value);
            }

            query = query.Include(itm => itm.Parent)
                .AsNoTracking();

            return await query.OrderBy(itm => itm.ClientId)
                .ThenBy(itm => itm.Parent)
                .ToListAsync();
        }

        public async Task RemoveAsync(IEnumerable<string> targetIds = null, IEnumerable<string> excludeKeys = null, IEnumerable<string> allowedClientIds = null)
        {
            var query = _context.Permissions.AsQueryable();
            if (targetIds != null && targetIds.Any())
            {
                query = query.Where(itm => targetIds.Contains(itm.Id));
            }

            if(excludeKeys != null && excludeKeys.Any())
            {
                query = query.Where(itm => !excludeKeys.Contains(itm.Key));
            }

            if (allowedClientIds != null && allowedClientIds.Any())
            {
                query = query.Where(itm => allowedClientIds.Contains(itm.ClientId));
            }

            var perms = await query.ToListAsync();
            _context.Permissions.RemoveRange(perms);
        }
    }
}
