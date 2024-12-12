using CitizensFinancialGroup.Threvw.Tenants.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    public class Privilege {
        public string Name { get; set; } // Name of the permission (e.g., "PERMISSION_1")
        public Visibility Visibility { get; set; } // Visibility settings
        public List<Condition> Conditions { get; set; } = new(); // Conditions associated with the privilege
    }
}
