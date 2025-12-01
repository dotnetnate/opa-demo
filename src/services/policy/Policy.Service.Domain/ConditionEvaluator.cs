using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace NOCO.Threvw.Policies.Domain {
    /// <summary>
    /// Evaluates policy conditions against context attributes.
    /// </summary>
    public interface IConditionEvaluator {
        /// <summary>
        /// Evaluates whether all conditions in the list are satisfied.
        /// </summary>
        bool EvaluateConditions(List<Condition> conditions, Dictionary<string, object> context);
    }

    public class ConditionEvaluator : IConditionEvaluator {
        private readonly ILogger<ConditionEvaluator> _logger;
        private readonly IValueComparer _valueComparer;

        public ConditionEvaluator(
            ILogger<ConditionEvaluator> logger,
            IValueComparer valueComparer) {
            _logger = logger;
            _valueComparer = valueComparer;
        }

        public bool EvaluateConditions(List<Condition> conditions, Dictionary<string, object> context) {
            if (conditions == null || conditions.Count == 0) {
                return true; // No conditions means always true
            }

            // All conditions must be satisfied (AND logic)
            foreach (var condition in conditions) {
                if (!EvaluateCondition(condition, context)) {
                    return false;
                }
            }

            return true;
        }

        private bool EvaluateCondition(Condition condition, Dictionary<string, object> context) {
            // Get value from context
            if (!context.TryGetValue(condition.ContextAttributePath, out var contextValue)) {
                _logger.LogDebug("Context attribute '{Attribute}' not found in request context",
                    condition.ContextAttributePath);
                return false;
            }

            try {
                return condition.Operator switch {
                    Operators.eq => _valueComparer.AreEqual(contextValue, condition.Value),
                    Operators.neq => !_valueComparer.AreEqual(contextValue, condition.Value),
                    Operators.gt => _valueComparer.Compare(contextValue, condition.Value) > 0,
                    Operators.gte => _valueComparer.Compare(contextValue, condition.Value) >= 0,
                    Operators.lt => _valueComparer.Compare(contextValue, condition.Value) < 0,
                    Operators.lte => _valueComparer.Compare(contextValue, condition.Value) <= 0,
                    _ => throw new InvalidOperationException($"Unknown operator: {condition.Operator}")
                };
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Error evaluating condition for attribute '{Attribute}'",
                    condition.ContextAttributePath);
                return false;
            }
        }
    }
}
