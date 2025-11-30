using System;
using System.Collections.Generic;
using System.Linq;

namespace CitizensFinancialGroup.Threvw.Policies.Domain.CombiningAlgorithms {
    /// <summary>
    /// Resolves the appropriate combining algorithm strategy for a given algorithm enum.
    /// </summary>
    public interface ICombiningAlgorithmStrategyResolver {
        /// <summary>
        /// Gets the strategy for the specified combining algorithm.
        /// </summary>
        ICombiningAlgorithmStrategy GetStrategy(CombiningAlgorithm algorithm);
    }

    public class CombiningAlgorithmStrategyResolver : ICombiningAlgorithmStrategyResolver {
        private readonly Dictionary<CombiningAlgorithm, ICombiningAlgorithmStrategy> _strategies;

        public CombiningAlgorithmStrategyResolver(IEnumerable<ICombiningAlgorithmStrategy> strategies) {
            _strategies = strategies.ToDictionary(s => s.Algorithm, s => s);
        }

        public ICombiningAlgorithmStrategy GetStrategy(CombiningAlgorithm algorithm) {
            if (_strategies.TryGetValue(algorithm, out var strategy)) {
                return strategy;
            }

            throw new InvalidOperationException(
                $"No strategy registered for combining algorithm: {algorithm}. " +
                $"To add support, create a new class implementing ICombiningAlgorithmStrategy and register it in DI.");
        }
    }
}
