using System.Collections.Generic;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class AssignPermissionToUserDto
    {
        public IEnumerable<string> PermissionIds { get; set; }

    }
}
