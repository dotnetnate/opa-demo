using System.ComponentModel;

namespace NOCO.Threvw.Policy.Service.Client {
    /// <summary>
    /// Defines how to combine results when multiple effect rules match.
    /// Based on XACML combining algorithms.
    /// </summary>
    public enum CombiningAlgorithm {
        /// <summary>
        /// First rule that applies (conditions match) determines the result.
        /// XACML: first-applicable
        /// </summary>
        [Description("firstApplicable")]
        FirstApplicable = 0,
        
        /// <summary>
        /// If any rule denies, result is Deny (unless no rules apply, then default).
        /// XACML: deny-overrides
        /// </summary>
        [Description("denyOverrides")]
        DenyOverrides = 1,
        
        /// <summary>
        /// If any rule permits, result is Permit (unless no rules apply, then default).
        /// XACML: permit-overrides
        /// </summary>
        [Description("permitOverrides")]
        PermitOverrides = 2,
        
        /// <summary>
        /// Exactly one rule must apply. Error if zero or multiple rules match.
        /// XACML: only-one-applicable
        /// </summary>
        [Description("onlyOneApplicable")]
        OnlyOneApplicable = 3
    }
}
