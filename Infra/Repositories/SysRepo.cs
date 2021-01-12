using System.Threading.Tasks;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Charlie.OpenIam.Infra.Repositories
{
    public class SysRepo : ISysRepo
    {
        private readonly ApplicationDbContext _context;

        public SysRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(SystemInfo model)
        {
            _context.SysInfo.Add(model);
        }

        public Task<SystemInfo> GetAsync(bool isReadonly = true)
        {
            return (isReadonly ? _context.SysInfo.AsNoTracking() : _context.SysInfo).FirstOrDefaultAsync();
        }
    }
}
