using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class AssignUserToOrgDto
    {
        public IEnumerable<string> UserIds { get; set; }
    }
}
