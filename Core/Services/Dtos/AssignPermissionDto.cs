using System.Collections.Generic;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class AssignPermissionDto
    {
        public IEnumerable<string> PermissionIds { get; set; }
    }
}
