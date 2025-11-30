using CitizensFinancialGroup.Threvw.Policies.Domain.CombiningAlgorithms;
using System.Collections.Generic;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Evaluates privileges by delegating to combining algorithm strategies.
    /// </summary>
    public interface IPrivilegeEvaluator {
        /// <summary>
        /// Evaluates a privilege and returns the decision with obligations and advice.
        /// </summary>
        PolicyEvaluationResult EvaluatePrivilege(
            Privilege privilege,
            Dictionary<string, object> context);
    }

    public class PrivilegeEvaluator : IPrivilegeEvaluator {
        private readonly IConditionEvaluator _conditionEvaluator;
        private readonly ICombiningAlgorithmStrategyResolver _strategyResolver;

        public PrivilegeEvaluator(
            IConditionEvaluator conditionEvaluator,
            ICombiningAlgorithmStrategyResolver strategyResolver) {
            _conditionEvaluator = conditionEvaluator;
            _strategyResolver = strategyResolver;
        }

        public PolicyEvaluationResult EvaluatePrivilege(
            Privilege privilege,
            Dictionary<string, object> context) {
            
            // Get the appropriate strategy and delegate evaluation
            var strategy = _strategyResolver.GetStrategy(privilege.CombiningAlgorithm);
            return strategy.Evaluate(privilege, context, _conditionEvaluator);
        }

    }
}
