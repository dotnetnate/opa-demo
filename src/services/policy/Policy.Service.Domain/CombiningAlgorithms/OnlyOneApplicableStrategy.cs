using System.Collections.Generic;
using System.Linq;

namespace NOCO.Threvw.Policies.Domain.CombiningAlgorithms {
    /// <summary>
    /// Only-One-Applicable combining algorithm.
    /// Exactly one rule must match. If zero or multiple rules match, returns Indeterminate.
    /// </summary>
    public class OnlyOneApplicableStrategy : ICombiningAlgorithmStrategy {
        public CombiningAlgorithm Algorithm => CombiningAlgorithm.OnlyOneApplicable;

        public PolicyEvaluationResult Evaluate(
            Privilege privilege,
            Dictionary<string, object> context,
            IConditionEvaluator conditionEvaluator) {
            
            var details = new List<string>();
            var applicableRules = GetApplicableRules(privilege, context, conditionEvaluator);
            int matchCount = applicableRules.Count;

            if (matchCount == 0) {
                details.Add("Only-one-applicable: No rules matched, using default");
                return new PolicyEvaluationResult {
                    Decision = ConvertEffect(privilege.DefaultEffect),
                    EvaluationDetails = string.Join("; ", details)
                };
            }

            if (matchCount > 1) {
                details.Add($"Only-one-applicable: ERROR - {matchCount} rules matched (expected exactly 1)");
                return new PolicyEvaluationResult {
                    Decision = PolicyDecision.Indeterminate,
                    EvaluationDetails = string.Join("; ", details)
                };
            }

            // Exactly one rule matched
            var matchingRule = applicableRules[0];
            details.Add("Only-one-applicable: Exactly one rule matched");
            return new PolicyEvaluationResult {
                Decision = ConvertEffect(matchingRule.Effect),
                Obligations = matchingRule.Obligations,
                Advice = matchingRule.Advice,
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
