using NOCO.Threvw.Policies.Domain;
using NOCO.Threvw.Policies.Domain.CombiningAlgorithms;

namespace Policy.Domain.Tests.Unit.CombiningAlgorithms {
    [TestClass]
    public class CombiningAlgorithmStrategyResolverTestFixture {
        [TestMethod]
        public void GetStrategy_FirstApplicable_ReturnsCorrectStrategy() {
            // Arrange
            var strategies = new List<ICombiningAlgorithmStrategy> {
                new FirstApplicableStrategy(),
                new DenyOverridesStrategy(),
                new PermitOverridesStrategy(),
                new OnlyOneApplicableStrategy()
            };
            var resolver = new CombiningAlgorithmStrategyResolver(strategies);

            // Act
            var strategy = resolver.GetStrategy(CombiningAlgorithm.FirstApplicable);

            // Assert
            Assert.IsInstanceOfType(strategy, typeof(FirstApplicableStrategy));
            Assert.AreEqual(CombiningAlgorithm.FirstApplicable, strategy.Algorithm);
        }

        [TestMethod]
        public void GetStrategy_DenyOverrides_ReturnsCorrectStrategy() {
            // Arrange
            var strategies = new List<ICombiningAlgorithmStrategy> {
                new FirstApplicableStrategy(),
                new DenyOverridesStrategy(),
                new PermitOverridesStrategy(),
                new OnlyOneApplicableStrategy()
            };
            var resolver = new CombiningAlgorithmStrategyResolver(strategies);

            // Act
            var strategy = resolver.GetStrategy(CombiningAlgorithm.DenyOverrides);

            // Assert
            Assert.IsInstanceOfType(strategy, typeof(DenyOverridesStrategy));
        }

        [TestMethod]
        public void GetStrategy_PermitOverrides_ReturnsCorrectStrategy() {
            // Arrange
            var strategies = new List<ICombiningAlgorithmStrategy> {
                new FirstApplicableStrategy(),
                new DenyOverridesStrategy(),
                new PermitOverridesStrategy(),
                new OnlyOneApplicableStrategy()
            };
            var resolver = new CombiningAlgorithmStrategyResolver(strategies);

            // Act
            var strategy = resolver.GetStrategy(CombiningAlgorithm.PermitOverrides);

            // Assert
            Assert.IsInstanceOfType(strategy, typeof(PermitOverridesStrategy));
        }

        [TestMethod]
        public void GetStrategy_OnlyOneApplicable_ReturnsCorrectStrategy() {
            // Arrange
            var strategies = new List<ICombiningAlgorithmStrategy> {
                new FirstApplicableStrategy(),
                new DenyOverridesStrategy(),
                new PermitOverridesStrategy(),
                new OnlyOneApplicableStrategy()
            };
            var resolver = new CombiningAlgorithmStrategyResolver(strategies);

            // Act
            var strategy = resolver.GetStrategy(CombiningAlgorithm.OnlyOneApplicable);

            // Assert
            Assert.IsInstanceOfType(strategy, typeof(OnlyOneApplicableStrategy));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetStrategy_UnknownAlgorithm_ThrowsException() {
            // Arrange
            var strategies = new List<ICombiningAlgorithmStrategy> {
                new FirstApplicableStrategy()
            };
            var resolver = new CombiningAlgorithmStrategyResolver(strategies);

            // Act - cast to invalid enum value
            resolver.GetStrategy((CombiningAlgorithm)999);
        }

        [TestMethod]
        public void Constructor_EmptyStrategies_CreatesResolver() {
            // Arrange
            var strategies = new List<ICombiningAlgorithmStrategy>();

            // Act
            var resolver = new CombiningAlgorithmStrategyResolver(strategies);

            // Assert
            Assert.IsNotNull(resolver);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_DuplicateStrategies_ThrowsException() {
            // Arrange - Two strategies with same algorithm
            var strategies = new List<ICombiningAlgorithmStrategy> {
                new FirstApplicableStrategy(),
                new FirstApplicableStrategy() // Duplicate
            };
            
            // Act - Should throw ArgumentException when duplicate keys are added
            var resolver = new CombiningAlgorithmStrategyResolver(strategies);
        }
    }
}
