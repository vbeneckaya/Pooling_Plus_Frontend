using System.Collections.Generic;

namespace Domain.Services.Roles
{
    public class RoleActionsDto
    {
        public IEnumerable<string> OrderActions { get; set; }
        public IEnumerable<string> ShippingActions { get; set; }
    }
}
