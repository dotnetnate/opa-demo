using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    public class Subject {
        public string Identifier { get; set; } // Unique identifier for the subject (e.g., user ID or role ID)
        public string Type { get; set; } // Type of subject (e.g., "user", "role")
        public string Authority { get; set; } // Authority governing the subject (e.g., "https://auth0.com/")
    }
}
