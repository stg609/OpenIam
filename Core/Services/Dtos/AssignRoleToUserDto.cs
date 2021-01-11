using System.Collections.Generic;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class AssignRoleToUserDto
    {
        public IEnumerable<string> RoleIds { get; set; }
    }
}
