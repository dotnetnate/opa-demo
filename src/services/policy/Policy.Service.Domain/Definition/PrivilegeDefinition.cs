using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Definition {
    public class PrivilegeDefinition {
        public string Name { get; set; } // Name of the permission (e.g., "PERMISSION_1")        
        public ICollection<ConditionDefinition> AllowedConditions { get; set; } = new List<ConditionDefinition>(); // List of conditions that are allowed for the privilege
    }
}
