using System.Collections.Generic;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Represents advisory information that MAY be acted upon by the policy enforcement point.
    /// Based on XACML Advice. Unlike obligations, advice can be ignored if the PEP cannot handle it.
    /// </summary>
    public class Advice {
        /// <summary>
        /// Type of advice (e.g., "audit", "monitor", "alert", "rateLimit").
        /// The enforcement point may choose to act on this or ignore it.
        /// </summary>
        public required string Type { get; set; }
        
        /// <summary>
        /// Additional parameters for the advice handler.
        /// Content depends on the advice type.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}
