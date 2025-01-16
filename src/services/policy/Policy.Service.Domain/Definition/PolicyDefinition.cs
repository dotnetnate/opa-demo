using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Definition {
    public class PolicyDefinition {
        public ICollection<PrivilegeDefinition> PrivilegeDefinitions { get; set; } = new List<PrivilegeDefinition>(); // List of rules that define the policy
    }
}
