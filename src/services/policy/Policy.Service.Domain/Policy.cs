using System.Collections.Generic;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {


    /// <summary>
    /// Represents a policy that contains a resource and a list of rules associated with it.
    /// </summary>
    public class Policy {
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the tenant id the policy belongs to.
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// Gets or sets the resource details.
        /// </summary>
        public required Resource Resource { get; set; }
        /// <summary>
        /// Gets or sets the list of rules associated with the resource.
        /// </summary>
        public required ICollection<Rule> Rules { get; set; } = [];
    }

}