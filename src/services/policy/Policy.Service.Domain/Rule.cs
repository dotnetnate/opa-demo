using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Represents a rule that associates a subject with a list of privileges.
    /// </summary>
    public class Rule {
        /// <summary>
        /// Gets or sets the subject details (user or role).
        /// </summary>
        public required Subject Subject { get; set; } // Subject details (user or role)

        /// <summary>
        /// Gets or sets the list of privileges granted to the subject.
        /// </summary>
        public List<Privilege> Privileges { get; set; } = new(); // List of privileges granted to the subject
    }
}
