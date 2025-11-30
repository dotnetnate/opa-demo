using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Orchestrates policy evaluation (Policy Decision Point).
    /// Responsible for finding policies and delegating evaluation logic.
    /// </summary>
    public class PolicyEvaluationService : IPolicyEvaluationService {
        private readonly IPolicyRepository _policyRepository;
        private readonly IPrivilegeEvaluator _privilegeEvaluator;
        private readonly ILogger<PolicyEvaluationService> _logger;

        public PolicyEvaluationService(
            IPolicyRepository policyRepository,
            IPrivilegeEvaluator privilegeEvaluator,
            ILogger<PolicyEvaluationService> logger) {
            _policyRepository = policyRepository;
            _privilegeEvaluator = privilegeEvaluator;
            _logger = logger;
        }

        public async Task<PolicyEvaluationResult> EvaluatePolicy(
            Resource resource,
            Subject subject,
            string actionId,
            Dictionary<string, object> context) {
            
            try {
                // Find policy for the resource
                var query = new FindPolicyByResourceQuery { Resource = resource };
                var policy = await _policyRepository.FindPolicyByResource(query);

                if (policy == null) {
                    _logger.LogWarning("No policy found for resource {ResourceId} ({ResourceType})",
                        resource.Identifier, resource.Authority);
                    return new PolicyEvaluationResult {
                        Decision = PolicyDecision.NotApplicable,
                        EvaluationDetails = "No policy found for resource"
                    };
                }

                // Find matching rule for the subject
                var matchingRule = policy.Rules?.FirstOrDefault(r =>
                    r.Subject.Identifier == subject.Identifier &&
                    r.Subject.Authority == subject.Authority);

                if (matchingRule == null) {
                    _logger.LogWarning("No matching rule found for subject {SubjectId} in policy for resource {ResourceId}",
                        subject.Identifier, resource.Identifier);
                    return new PolicyEvaluationResult {
                        Decision = PolicyDecision.Deny,
                        EvaluationDetails = "No rule found for subject"
                    };
                }

                // Find privilege for the action
                var privilege = matchingRule.Privileges?.FirstOrDefault(p =>
                    p.PermissionName.Equals(actionId, StringComparison.OrdinalIgnoreCase));

                if (privilege == null) {
                    _logger.LogWarning("No privilege found for action {ActionId} in rule for subject {SubjectId}",
                        actionId, subject.Identifier);
                    return new PolicyEvaluationResult {
                        Decision = PolicyDecision.Deny,
                        EvaluationDetails = $"No privilege defined for action '{actionId}'"
                    };
                }

                // Delegate evaluation to the privilege evaluator
                return _privilegeEvaluator.EvaluatePrivilege(privilege, context);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error evaluating policy for resource {ResourceId}, subject {SubjectId}, action {ActionId}",
                    resource.Identifier, subject.Identifier, actionId);
                return new PolicyEvaluationResult {
                    Decision = PolicyDecision.Indeterminate,
                    EvaluationDetails = $"Error during evaluation: {ex.Message}"
                };
            }
        }
    }
}
