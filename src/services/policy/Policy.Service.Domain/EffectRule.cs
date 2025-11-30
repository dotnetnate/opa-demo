using CitizensFinancialGroup.Elements;
using System;
using System.Collections.Generic;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Represents a rule that specifies an effect (Permit/Deny) along with conditions,
    /// obligations, and advice. Multiple effect rules can exist within a privilege.
    /// </summary>
    public class EffectRule {
        /// <summary>
        /// The effect to apply if conditions are met: Permit or Deny.
        /// </summary>
        public PermissionActions Effect { get; set; }
        
        /// <summary>
        /// Conditions that must be satisfied for this rule to apply.
        /// Empty list means unconditional (always matches).
        /// </summary>
        public List<Condition> Conditions { get; set; } = new();
        
        /// <summary>
        /// Validity period for this rule. If null, rule is always temporally valid.
        /// </summary>
        public Range<DateTimeOffset>? ValidityPeriod { get; set; }
        
        /// <summary>
        /// Obligations that MUST be fulfilled if this rule's effect is applied.
        /// If obligations cannot be fulfilled, the effect should not be enforced.
        /// </summary>
        public List<Obligation> Obligations { get; set; } = new();
        
        /// <summary>
        /// Advisory information that MAY be considered by the enforcement point.
        /// Unlike obligations, advice can be safely ignored.
        /// </summary>
        public List<Advice> Advice { get; set; } = new();
        
        /// <summary>
        /// Indicates if the validity period has lapsed.
        /// </summary>
        public bool HasValidityPeriodLapsed() {
            return ValidityPeriod != null && DateTimeOffset.Now.CompareTo(ValidityPeriod.End) > 0;
        }
    }
}
