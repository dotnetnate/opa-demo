using NOCO.Threvw.Policies.Domain;
using NOCO.Threvw.Policies.Domain.CombiningAlgorithms;
using Moq;

namespace Policy.Domain.Tests.Unit {
    [TestClass]
    public class PrivilegeEvaluatorTestFixture {
        private Mock<IConditionEvaluator> _mockConditionEvaluator = null!;
        private Mock<ICombiningAlgorithmStrategyResolver> _mockStrategyResolver = null!;
        private Mock<ICombiningAlgorithmStrategy> _mockStrategy = null!;
        private PrivilegeEvaluator _evaluator = null!;

        [TestInitialize]
        public void Setup() {
            _mockConditionEvaluator = new Mock<IConditionEvaluator>();
            _mockStrategyResolver = new Mock<ICombiningAlgorithmStrategyResolver>();
            _mockStrategy = new Mock<ICombiningAlgorithmStrategy>();
            _evaluator = new PrivilegeEvaluator(
                _mockConditionEvaluator.Object,
                _mockStrategyResolver.Object);
        }

        [TestMethod]
        public void EvaluatePrivilege_DelegatesToStrategyResolver() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                CombiningAlgorithm = CombiningAlgorithm.FirstApplicable,
                EffectRules = new List<EffectRule>(),
                DefaultEffect = PermissionActions.Deny
            };
            var context = new Dictionary<string, object>();
            var expectedResult = new PolicyEvaluationResult {
                Decision = PolicyDecision.Permit
            };

            _mockStrategyResolver
                .Setup(r => r.GetStrategy(CombiningAlgorithm.FirstApplicable))
                .Returns(_mockStrategy.Object);

            _mockStrategy
                .Setup(s => s.Evaluate(privilege, context, _mockConditionEvaluator.Object))
                .Returns(expectedResult);

            // Act
            var result = _evaluator.EvaluatePrivilege(privilege, context);

            // Assert
            Assert.AreEqual(PolicyDecision.Permit, result.Decision);
            _mockStrategyResolver.Verify(
                r => r.GetStrategy(CombiningAlgorithm.FirstApplicable),
                Times.Once);
            _mockStrategy.Verify(
                s => s.Evaluate(privilege, context, _mockConditionEvaluator.Object),
                Times.Once);
        }

        [TestMethod]
        public void EvaluatePrivilege_DifferentAlgorithms_CallsCorrectStrategy() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                CombiningAlgorithm = CombiningAlgorithm.DenyOverrides,
                EffectRules = new List<EffectRule>(),
                DefaultEffect = PermissionActions.Deny
            };
            var context = new Dictionary<string, object>();
            var expectedResult = new PolicyEvaluationResult {
                Decision = PolicyDecision.Deny
            };

            _mockStrategyResolver
                .Setup(r => r.GetStrategy(CombiningAlgorithm.DenyOverrides))
                .Returns(_mockStrategy.Object);

            _mockStrategy
                .Setup(s => s.Evaluate(privilege, context, _mockConditionEvaluator.Object))
                .Returns(expectedResult);

            // Act
            var result = _evaluator.EvaluatePrivilege(privilege, context);

            // Assert
            Assert.AreEqual(PolicyDecision.Deny, result.Decision);
            _mockStrategyResolver.Verify(
                r => r.GetStrategy(CombiningAlgorithm.DenyOverrides),
                Times.Once);
        }

        [TestMethod]
        public void EvaluatePrivilege_ReturnsObligationsFromStrategy() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                CombiningAlgorithm = CombiningAlgorithm.FirstApplicable,
                EffectRules = new List<EffectRule>(),
                DefaultEffect = PermissionActions.Deny
            };
            var context = new Dictionary<string, object>();
            var expectedResult = new PolicyEvaluationResult {
                Decision = PolicyDecision.Permit,
                Obligations = new List<Obligation> {
                    new Obligation { Type = "approval" }
                },
                Advice = new List<Advice> {
                    new Advice { Type = "monitor" }
                }
            };

            _mockStrategyResolver
                .Setup(r => r.GetStrategy(CombiningAlgorithm.FirstApplicable))
                .Returns(_mockStrategy.Object);

            _mockStrategy
                .Setup(s => s.Evaluate(privilege, context, _mockConditionEvaluator.Object))
                .Returns(expectedResult);

            // Act
            var result = _evaluator.EvaluatePrivilege(privilege, context);

            // Assert
            Assert.AreEqual(PolicyDecision.Permit, result.Decision);
            Assert.AreEqual(1, result.Obligations.Count);
            Assert.AreEqual("approval", result.Obligations[0].Type);
            Assert.AreEqual(1, result.Advice.Count);
            Assert.AreEqual("monitor", result.Advice[0].Type);
        }
    }
}
