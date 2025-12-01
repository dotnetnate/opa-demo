using NOCO.Threvw.Policies.Domain;
using NOCO.Threvw.Policies.Domain.CombiningAlgorithms;
using Moq;

namespace Policy.Domain.Tests.Unit.CombiningAlgorithms {
    [TestClass]
    public class PermitOverridesStrategyTestFixture {
        private PermitOverridesStrategy _strategy = null!;
        private Mock<IConditionEvaluator> _mockEvaluator = null!;

        [TestInitialize]
        public void Setup() {
            _strategy = new PermitOverridesStrategy();
            _mockEvaluator = new Mock<IConditionEvaluator>();
        }

        [TestMethod]
        public void Algorithm_ReturnsPermitOverrides() {
            // Assert
            Assert.AreEqual(CombiningAlgorithm.PermitOverrides, _strategy.Algorithm);
        }

        [TestMethod]
        public void Evaluate_PermitAndDenyRules_PermitWins() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Deny,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "notification" } }
                    },
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "logging" } }
                    }
                },
                DefaultEffect = PermissionActions.Deny
            };
            var context = new Dictionary<string, object>();

            _mockEvaluator.Setup(e => e.EvaluateConditions(It.IsAny<List<Condition>>(), context))
                .Returns(true);

            // Act
            var result = _strategy.Evaluate(privilege, context, _mockEvaluator.Object);

            // Assert
            Assert.AreEqual(PolicyDecision.Permit, result.Decision);
            Assert.AreEqual(1, result.Obligations.Count);
            Assert.AreEqual("logging", result.Obligations[0].Type);
            Assert.IsTrue(result.EvaluationDetails!.Contains("Permit-overrides: Found permitting rule"));
        }

        [TestMethod]
        public void Evaluate_OnlyDenyRules_ReturnsDeny() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Deny,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "notification" } }
                    }
                },
                DefaultEffect = PermissionActions.Permit
            };
            var context = new Dictionary<string, object>();

            _mockEvaluator.Setup(e => e.EvaluateConditions(It.IsAny<List<Condition>>(), context))
                .Returns(true);

            // Act
            var result = _strategy.Evaluate(privilege, context, _mockEvaluator.Object);

            // Assert
            Assert.AreEqual(PolicyDecision.Deny, result.Decision);
            Assert.AreEqual(1, result.Obligations.Count);
            Assert.IsTrue(result.EvaluationDetails!.Contains("no permitting rules"));
        }

        [TestMethod]
        public void Evaluate_MultiplePermitRules_ReturnsFirstPermit() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "first" } }
                    },
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "second" } }
                    }
                },
                DefaultEffect = PermissionActions.Deny
            };
            var context = new Dictionary<string, object>();

            _mockEvaluator.Setup(e => e.EvaluateConditions(It.IsAny<List<Condition>>(), context))
                .Returns(true);

            // Act
            var result = _strategy.Evaluate(privilege, context, _mockEvaluator.Object);

            // Assert
            Assert.AreEqual(PolicyDecision.Permit, result.Decision);
            Assert.AreEqual("first", result.Obligations[0].Type);
        }
    }
}
