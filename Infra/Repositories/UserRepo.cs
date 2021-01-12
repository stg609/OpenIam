using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Charlie.OpenIam.Infra.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;

        public UserRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsExistedAsync(string id = null, string jobNo = null, string phone = null)
        {
            var query = _context.Users.AsNoTracking();

            if (!String.IsNullOrWhiteSpace(id))
            {
                query = query.Where(itm => itm.Id == id);
            }

            if (!String.IsNullOrWhiteSpace(jobNo))
            {
                query = query.Where(itm => itm.JobNo == jobNo);
            }

            if (!String.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(itm => itm.PhoneNumber == phone);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsJobNoUniqueAsync()
        {
            return !await _context.Users.Where(itm => !String.IsNullOrWhiteSpace(itm.JobNo)).GroupBy(itm => itm.JobNo).AnyAsync(itm => itm.Count() > 1);
        }

        public async Task<bool> IsPhoneUniqueAsync()
        {
            return !await _context.Users.Where(itm => !String.IsNullOrWhiteSpace(itm.PhoneNumber)).GroupBy(itm => itm.PhoneNumber).AnyAsync(itm => itm.Count() > 1);
        }

        public async Task<PaginatedDto<ApplicationUser>> GetAllAsync(string firstName = null, string lastName = null, string jobNo = null, string idcard = null, string phone = null, string email = null, string excludeOrgId = null, bool? isActive = null, int pageSize = 10, int pageIndex = 0)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            List<ApplicationUser> users;
            var query = _context.Users.AsNoTracking();

            if (isActive.HasValue)
            {
                if (isActive.Value)
                {
                    query = query.Where(itm => itm.IsActive);
                }
                else
                {
                    query = query.Where(itm => !itm.IsActive);
                }
            }

            if (!String.IsNullOrWhiteSpace(firstName))
            {
                query = query.Where(itm => itm.FirstName.Contains(firstName));
            }

            if (!String.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(itm => itm.LastName.Contains(lastName));
            }

            if (!String.IsNullOrWhiteSpace(idcard))
            {
                query = query.Where(itm => itm.IdCard.Contains(idcard));
            }

            if (!String.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(itm => itm.PhoneNumber.Contains(phone));
            }

            if (!String.IsNullOrWhiteSpace(email))
            {
                query = query.Where(itm => EF.Functions.ILike(itm.Email, $"%{email}%"));
            }

            if (!String.IsNullOrWhiteSpace(jobNo))
            {
                query = query.Where(itm => itm.JobNo.Contains(jobNo));
            }

            if (!String.IsNullOrWhiteSpace(excludeOrgId))
            {
                query = query.Where(itm => !itm.UserOrganizations.Any(uo => uo.OrganizationId == excludeOrgId));
            }

            query = query.Include(itm => itm.UserOrganizations)
                        .ThenInclude(itm => itm.Organization);

            if (pageSize <= 0)
            {
                // 不分页
                users = await query
                    .ToListAsync();
            }
            else
            {
                users = await query
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                    .ToListAsync();
            }

            var total = await query.CountAsync();

            return new PaginatedDto<ApplicationUser>
            {
                Data = users,
                Total = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<ApplicationUser> GetAsync(string id = null, string jobNo = null, string phone = null, bool isReadonly = true)
        {
            var query = isReadonly ? _context.Users.AsNoTracking() :
                _context.Users;

            if (!String.IsNullOrWhiteSpace(id))
            {
                query = query.Where(itm => itm.Id == id);
            }

            if (!String.IsNullOrWhiteSpace(jobNo))
            {
                query = query.Where(itm => itm.JobNo == jobNo);
            }

            if (!String.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(itm => itm.PhoneNumber == phone);
            }

            var user = await query
              .Include(itm => itm.UserPermissions)
                    .ThenInclude(itm => itm.Permission)
                        .ThenInclude(itm => itm.Parent)
              .Include(itm => itm.UserOrganizations)
                   .ThenInclude(itm => itm.Organization)
                       .ThenInclude(itm => itm.OrganizationRoles)
                           .ThenInclude(itm => itm.Role)
              .FirstOrDefaultAsync();

            return user;
        }
    }
}
