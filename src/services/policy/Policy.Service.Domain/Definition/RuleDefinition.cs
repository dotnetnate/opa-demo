using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Definition {
    public class RuleDefinition {
        public ICollection<PrivilegeDefinition> PrivilegeDefinitions { get; set; } = new List<PrivilegeDefinition>(); // List of privileges granted to the subject
    }
}
