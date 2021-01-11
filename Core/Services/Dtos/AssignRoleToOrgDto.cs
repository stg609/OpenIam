using System.Collections.Generic;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class AssignRoleToOrgDto
    {
        public IEnumerable<string> RoleIds { get; set; }
    }
}
