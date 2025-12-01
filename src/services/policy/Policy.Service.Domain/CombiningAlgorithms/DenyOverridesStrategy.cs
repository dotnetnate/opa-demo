using System.Collections.Generic;
using System.Linq;

namespace NOCO.Threvw.Policies.Domain.CombiningAlgorithms {
    /// <summary>
    /// Deny-Overrides combining algorithm.
    /// If any rule denies, the result is Deny. Otherwise, if any permits, result is Permit.
    /// </summary>
    public class DenyOverridesStrategy : ICombiningAlgorithmStrategy {
        public CombiningAlgorithm Algorithm => CombiningAlgorithm.DenyOverrides;

        public PolicyEvaluationResult Evaluate(
            Privilege privilege,
            Dictionary<string, object> context,
            IConditionEvaluator conditionEvaluator) {
            
            var details = new List<string>();
            var applicableRules = GetApplicableRules(privilege, context, conditionEvaluator);

            // If any rule denies, result is Deny
            var denyRule = applicableRules.FirstOrDefault(r => r.Effect == PermissionActions.Deny);
            if (denyRule != null) {
                details.Add("Deny-overrides: Found denying rule");
                return new PolicyEvaluationResult {
                    Decision = PolicyDecision.Deny,
                    Obligations = denyRule.Obligations,
                    Advice = denyRule.Advice,
                    EvaluationDetails = string.Join("; ", details)
                };
            }

            // Otherwise, if any rule permits, result is Permit
            var permitRule = applicableRules.FirstOrDefault(r => r.Effect == PermissionActions.Permit);
            if (permitRule != null) {
                details.Add("Deny-overrides: Found permitting rule, no denying rules");
                return new PolicyEvaluationResult {
                    Decision = PolicyDecision.Permit,
                    Obligations = permitRule.Obligations,
                    Advice = permitRule.Advice,
                    EvaluationDetails = string.Join("; ", details)
                };
            }

            // No rules matched, use default
            details.Add($"Deny-overrides: No rules matched, using default: {privilege.DefaultEffect}");
            return new PolicyEvaluationResult {
                Decision = ConvertEffect(privilege.DefaultEffect),
                EvaluationDetails = string.Join("; ", details)
            };
        }

        private List<EffectRule> GetApplicableRules(
            Privilege privilege,
            Dictionary<string, object> context,
            IConditionEvaluator conditionEvaluator) {
            
            var applicableRules = new List<EffectRule>();

            foreach (var effectRule in privilege.EffectRules ?? Enumerable.Empty<EffectRule>()) {
                if (effectRule.HasValidityPeriodLapsed()) {
                    continue;
                }

                if (conditionEvaluator.EvaluateConditions(effectRule.Conditions, context)) {
                    applicableRules.Add(effectRule);
                }
            }

            return applicableRules;
        }

        private PolicyDecision ConvertEffect(PermissionActions effect) {
            return effect == PermissionActions.Permit
                ? PolicyDecision.Permit
                : PolicyDecision.Deny;
        }
    }
}
