using System.Collections.Generic;

namespace NOCO.Threvw.Policies.Domain {
    /// <summary>
    /// Represents an obligation that MUST be fulfilled when a policy decision is enforced.
    /// Based on XACML Obligations. If obligations cannot be fulfilled, the decision should be denied.
    /// </summary>
    public class Obligation {
        /// <summary>
        /// Type of obligation (e.g., "approval", "mfa", "logging", "notification").
        /// The enforcement point uses this to determine which handler to invoke.
        /// </summary>
        public required string Type { get; set; }
        
        /// <summary>
        /// Additional parameters for the obligation handler.
        /// Content depends on the obligation type.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}
