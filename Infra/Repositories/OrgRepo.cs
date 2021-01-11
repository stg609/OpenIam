using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Charlie.OpenIam.Infra.Repositories
{
    public class OrgRepo : IOrgRepo
    {
        private readonly ApplicationDbContext _context;

        public OrgRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Organization organization)
        {
            _context.Organizations.Add(organization);
        }

        public async Task<IEnumerable<Organization>> GetAllAsync(string name = null, bool? isEnabled = null)
        {
            var query = _context.Organizations.AsNoTracking();

            if (!String.IsNullOrWhiteSpace(name))
            {
                query = query.Where(itm => itm.Name.Contains(name));
            }

            if (isEnabled.HasValue)
            {
                query = query.Where(itm => itm.Enabled == isEnabled);
            }

            var orgs = await query.Include(itm => itm.Parent)
                .Include(itm => itm.OrganizationRoles)
                    .ThenInclude(itm => itm.Role)
               .ToListAsync();

            return orgs;
        }

        public async Task<Organization> GetAsync(string id, bool isReadonly = true)
        {
            var query = isReadonly ? _context.Organizations.AsNoTracking() :
                _context.Organizations;

            var org = await query
                .Include(itm => itm.OrganizationRoles)
                    .ThenInclude(itm => itm.Role)
                .Include(itm => itm.UserOrganizations)
                    .ThenInclude(itm => itm.User)
                .Include(itm => itm.Parent)
                .SingleOrDefaultAsync(x => x.Id == id);
            return org;
        }

        public async Task<IEnumerable<string>> RemoveAsync(string userId, IEnumerable<string> targetIds)
        {
            if (targetIds == null || !targetIds.Any())
            {
                return Enumerable.Empty<string>();
            }

            var orgs = await _context.Organizations
                .Include(itm => itm.UserOrganizations)
                .Where(x => targetIds.Any(itm => itm == x.Id))
                .ToListAsync();
            if (orgs == null)
            {
                return targetIds;
            }

            // 如果组织机构里面只包含当前用户自己，那才可以删除
            var canDelete = orgs.Where(itm => itm.UserOrganizations == null || !itm.UserOrganizations.Any() || (itm.UserOrganizations.Count == 1 && itm.UserOrganizations.Any(uo => uo.UserId == userId)));

            if (canDelete.Any())
            {
                // 排除包含子组织的组织
                var canDeleteOrgIds = canDelete.Select(itm => itm.Id);
                var orgsHaveChildren = await _context.Organizations.Where(itm => canDeleteOrgIds.Contains(itm.Parent.Id)).ToListAsync();
                canDelete = canDelete.Except(orgsHaveChildren);

                _context.Organizations.RemoveRange(canDelete);
                return canDelete.Select(itm => itm.Id);
            }

            return Enumerable.Empty<string>();
        }

        public async Task RemoveDefaultRolesAsync()
        {

        }
    }
}
