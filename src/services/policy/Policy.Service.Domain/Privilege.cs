using NOCO.Threvw.Tenants.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NOCO.Elements;

namespace NOCO.Threvw.Policies.Domain {
    /// <summary>
    /// Represents a privilege with a permission name and multiple effect rules.
    /// Based on XACML policy model with support for conditional effects, obligations, and advice.
    /// </summary>
    public class Privilege {
        /// <summary>
        /// Gets or sets the name of the permission (e.g., "READ", "WRITE", "WIRE_TRANSFER").
        /// </summary>
        public required string PermissionName { get; set; }
        
        /// <summary>
        /// How to combine results when multiple effect rules match.
        /// Defaults to FirstApplicable (first matching rule wins).
        /// </summary>
        public CombiningAlgorithm CombiningAlgorithm { get; set; } = CombiningAlgorithm.FirstApplicable;
        
        /// <summary>
        /// List of effect rules evaluated according to the combining algorithm.
        /// Each rule specifies an effect (Permit/Deny) with optional conditions, obligations, and advice.
        /// </summary>
        public List<EffectRule> EffectRules { get; set; } = new();
        
        /// <summary>
        /// Default effect if no rules match. Defaults to Deny for secure-by-default behavior.
        /// </summary>
        public PermissionActions DefaultEffect { get; set; } = PermissionActions.Deny;
    }
}
