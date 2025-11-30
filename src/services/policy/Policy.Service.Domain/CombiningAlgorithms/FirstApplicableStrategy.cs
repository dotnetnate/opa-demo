using System.Collections.Generic;
using System.Linq;

namespace CitizensFinancialGroup.Threvw.Policies.Domain.CombiningAlgorithms {
    /// <summary>
    /// First-Applicable combining algorithm.
    /// The first rule whose conditions match determines the result.
    /// </summary>
    public class FirstApplicableStrategy : ICombiningAlgorithmStrategy {
        public CombiningAlgorithm Algorithm => CombiningAlgorithm.FirstApplicable;

        public PolicyEvaluationResult Evaluate(
            Privilege privilege,
            Dictionary<string, object> context,
            IConditionEvaluator conditionEvaluator) {
            
            var details = new List<string>();

            // First rule that applies (conditions match) determines the result
            foreach (var effectRule in privilege.EffectRules ?? Enumerable.Empty<EffectRule>()) {
                if (effectRule.HasValidityPeriodLapsed()) {
                    details.Add("Rule skipped: validity period lapsed");
                    continue;
                }

                if (conditionEvaluator.EvaluateConditions(effectRule.Conditions, context)) {
                    details.Add($"First applicable rule matched with effect: {effectRule.Effect}");
                    return new PolicyEvaluationResult {
                        Decision = ConvertEffect(effectRule.Effect),
                        Obligations = effectRule.Obligations,
                        Advice = effectRule.Advice,
                        EvaluationDetails = string.Join("; ", details)
                    };
                }
            }

            // No rules matched, use default effect
            details.Add($"No rules matched, using default effect: {privilege.DefaultEffect}");
            return new PolicyEvaluationResult {
                Decision = ConvertEffect(privilege.DefaultEffect),
                EvaluationDetails = string.Join("; ", details)
            };
        }

        private PolicyDecision ConvertEffect(PermissionActions effect) {
            return effect == PermissionActions.Permit
                ? PolicyDecision.Permit
                : PolicyDecision.Deny;
        }
    }
}
