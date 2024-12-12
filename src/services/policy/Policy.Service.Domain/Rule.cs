using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    public class Rule {
        public Subject Subject { get; set; } // Subject details (user or role)
        public List<Privilege> Privileges { get; set; } = new(); // List of privileges granted to the subject
    }
}
