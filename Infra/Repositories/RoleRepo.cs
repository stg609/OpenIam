using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Charlie.OpenIam.Infra.Repositories
{
    public class RoleRepo : IRoleRepo
    {
        private readonly ApplicationDbContext _context;

        public RoleRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedDto<ApplicationRole>> GetAllAsync(string name = null, string clientId = null, IEnumerable<string> roleIds = null, bool withPerms = false, IEnumerable<string> allowedClientIds = null, int pageSize = 10, int pageIndex = 1)
        {
            var query = _context.Roles.AsNoTracking();

            if (allowedClientIds != null)
            {
                if (!String.IsNullOrWhiteSpace(clientId) && !allowedClientIds.Contains(clientId))
                {
                    throw new IamException(HttpStatusCode.BadRequest, "无权操作!");
                }

                query = query.Where(itm => allowedClientIds.Contains(itm.ClientId));
            }

            if (!String.IsNullOrWhiteSpace(clientId))
            {
                query = query.Where(itm => itm.ClientId.Contains(clientId));
            }

            if (!String.IsNullOrWhiteSpace(name))
            {
                query = query.Where(itm => EF.Functions.ILike(itm.Name, $"%{name}%"));
            }

            if (roleIds != null)
            {
                query = query.Where(itm => roleIds.Contains(itm.Id));
            }

            var includeQuery = withPerms ? query
                    .Include(itm => itm.Permissions)
                        .ThenInclude(itm => itm.Permission) :
                        query;

            query = includeQuery
               .OrderBy(itm => itm.ClientId)
               .ThenByDescending(itm => itm.CreatedAt);

            IEnumerable<ApplicationRole> roles;
            if (pageSize > 0)
            {
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                roles = await query
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
            }
            else
            {
                roles = await query.ToListAsync();
            }
            int total = await query.CountAsync();

            return new PaginatedDto<ApplicationRole>
            {
                Data = roles,
                Total = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllByNamesAsync(IEnumerable<string> names, IEnumerable<string> allowedClientIds = null)
        {
            var query = _context.Roles.AsNoTracking();

            if (allowedClientIds != null)
            {
                query = query.Where(itm => String.IsNullOrWhiteSpace(itm.ClientId) || allowedClientIds.Contains(itm.ClientId));
            }

            if (names != null)
            {
                query = query.Where(itm => names.Contains(itm.Name));
            }
            return await query
                .Include(itm => itm.Permissions)
                    .ThenInclude(itm => itm.Permission)
                        .ThenInclude(itm => itm.Parent)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllByIdsAsync(IEnumerable<string> ids, IEnumerable<string> allowedClientIds = null)
        {
            var query = _context.Roles.AsNoTracking()
                .Where(itm => String.IsNullOrWhiteSpace(itm.ClientId));

            if (allowedClientIds != null)
            {
                query = query.Where(itm => allowedClientIds.Contains(itm.ClientId));
            }

            if (ids != null)
            {
                query = query.Where(itm => ids.Contains(itm.Id));
            }
            return await query
                .Include(itm => itm.Permissions)
                    .ThenInclude(itm => itm.Permission)
                        .ThenInclude(itm => itm.Parent)
                .ToListAsync();
        }

        public async Task<ApplicationRole> GetAsync(string id, bool withPerms = false, bool isReadonly = true)
        {
            ApplicationRole role;

            var query = isReadonly ? _context.Roles.AsNoTracking() :
                _context.Roles;
            if (withPerms)
            {
                role = await query
                    .Include(itm => itm.Permissions)
                        .ThenInclude(itm => itm.Permission)
                    .SingleOrDefaultAsync(itm => itm.Id == id);
            }
            else
            {
                role = await query.SingleOrDefaultAsync(itm => itm.Id == id);
            }

            return role;
        }
        public async Task<bool> IsExistedAsync(string name = null, string clientId = null)
        {
            var query = _context.Roles.AsNoTracking();
            if (!String.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => EF.Functions.ILike(x.Name, $"%{name}%"));
            }

            if (!String.IsNullOrWhiteSpace(clientId))
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            return await query.AnyAsync();
        }
    }
}
