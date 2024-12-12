using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    public class Condition {
        public string Attribute { get; set; } // Attribute name (e.g., "amount")
        public Operator Operator { get; set; } // Operator as an enumeration (e.g., "eq", "gte")
        public object Value { get; set; } // Value to compare against
    }
}
