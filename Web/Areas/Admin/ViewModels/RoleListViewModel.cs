using System.Collections.Generic;
using Charlie.OpenIam.Abstraction.Dtos;

namespace Charlie.OpenIam.Web.Areas.Admin.ViewModels
{
    public class RoleListViewModel
    {
        public IEnumerable<RoleDto> Roles { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
