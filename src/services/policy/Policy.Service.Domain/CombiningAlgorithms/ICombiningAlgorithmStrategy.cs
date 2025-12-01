using System.Collections.Generic;

namespace NOCO.Threvw.Policies.Domain.CombiningAlgorithms {
    /// <summary>
    /// Strategy interface for evaluating privileges using different combining algorithms.
    /// Implementing classes define how multiple effect rules are combined.
    /// </summary>
    public interface ICombiningAlgorithmStrategy {
        /// <summary>
        /// The combining algorithm this strategy handles.
        /// </summary>
        CombiningAlgorithm Algorithm { get; }

        /// <summary>
        /// Evaluates a privilege using the combining algorithm.
        /// </summary>
        /// <param name="privilege">The privilege to evaluate.</param>
        /// <param name="context">The evaluation context.</param>
        /// <param name="conditionEvaluator">The condition evaluator to use.</param>
        /// <returns>The evaluation result with decision, obligations, and advice.</returns>
        PolicyEvaluationResult Evaluate(
            Privilege privilege,
            Dictionary<string, object> context,
            IConditionEvaluator conditionEvaluator);
    }
}
