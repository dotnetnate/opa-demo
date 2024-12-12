using System.Collections.Generic;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {

    public class Policy {
        public Resource Resource { get; set; } // Resource details
        public List<Rule> Rules { get; set; } = new(); // A list of rules associated with the resource
    }

}