using System.Collections.Generic;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {
    /// <summary>
    /// Service for evaluating policy decisions (Policy Decision Point - PDP).
    /// </summary>
    public interface IPolicyEvaluationService {
        /// <summary>
        /// Evaluates a policy decision for a given resource, subject, and action context.
        /// </summary>
        /// <param name="resource">The resource being accessed.</param>
        /// <param name="subject">The subject requesting access.</param>
        /// <param name="actionId">The action being performed (e.g., "READ", "WRITE").</param>
        /// <param name="context">Additional context for condition evaluation.</param>
        /// <returns>A policy evaluation result with decision, obligations, and advice.</returns>
        Task<PolicyEvaluationResult> EvaluatePolicy(
            Resource resource,
            Subject subject,
            string actionId,
            Dictionary<string, object> context);
    }

    /// <summary>
    /// Result of a policy evaluation.
    /// </summary>
    public class PolicyEvaluationResult {
        /// <summary>
        /// The final decision: Permit or Deny.
        /// </summary>
        public PolicyDecision Decision { get; set; }

        /// <summary>
        /// Obligations that MUST be fulfilled for this decision to be enforced.
        /// </summary>
        public List<Obligation> Obligations { get; set; } = new();

        /// <summary>
        /// Advisory information that MAY be acted upon.
        /// </summary>
        public List<Advice> Advice { get; set; } = new();

        /// <summary>
        /// Additional details about the evaluation (for debugging/audit).
        /// </summary>
        public string? EvaluationDetails { get; set; }
    }

    /// <summary>
    /// Policy decision enum compatible with XACML.
    /// </summary>
    public enum PolicyDecision {
        /// <summary>
        /// Access is denied.
        /// </summary>
        Deny = 0,

        /// <summary>
        /// Access is permitted (subject to obligations being fulfilled).
        /// </summary>
        Permit = 1,

        /// <summary>
        /// Policy evaluation encountered an error.
        /// </summary>
        Indeterminate = 2,

        /// <summary>
        /// No applicable policy was found.
        /// </summary>
        NotApplicable = 3
    }
}
