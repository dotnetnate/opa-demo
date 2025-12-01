using NOCO.Threvw.Policies.Domain;
using NOCO.Threvw.Policies.Domain.CombiningAlgorithms;
using Moq;

namespace Policy.Domain.Tests.Unit.CombiningAlgorithms {
    [TestClass]
    public class DenyOverridesStrategyTestFixture {
        private DenyOverridesStrategy _strategy = null!;
        private Mock<IConditionEvaluator> _mockEvaluator = null!;

        [TestInitialize]
        public void Setup() {
            _strategy = new DenyOverridesStrategy();
            _mockEvaluator = new Mock<IConditionEvaluator>();
        }

        [TestMethod]
        public void Algorithm_ReturnsDenyOverrides() {
            // Assert
            Assert.AreEqual(CombiningAlgorithm.DenyOverrides, _strategy.Algorithm);
        }

        [TestMethod]
        public void Evaluate_OnePermitRule_ReturnsPermit() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
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
        }

        [TestMethod]
        public void Evaluate_OneDenyRule_ReturnsDeny() {
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
        }

        [TestMethod]
        public void Evaluate_PermitAndDenyRules_DenyWins() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "logging" } }
                    },
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
            Assert.AreEqual("notification", result.Obligations[0].Type);
            Assert.IsTrue(result.EvaluationDetails!.Contains("Deny-overrides: Found denying rule"));
        }

        [TestMethod]
        public void Evaluate_NoRulesMatch_ReturnsDefault() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>()
                    }
                },
                DefaultEffect = PermissionActions.Deny
            };
            var context = new Dictionary<string, object>();

            _mockEvaluator.Setup(e => e.EvaluateConditions(It.IsAny<List<Condition>>(), context))
                .Returns(false);

            // Act
            var result = _strategy.Evaluate(privilege, context, _mockEvaluator.Object);

            // Assert
            Assert.AreEqual(PolicyDecision.Deny, result.Decision);
            Assert.IsTrue(result.EvaluationDetails!.Contains("No rules matched"));
        }

        [TestMethod]
        public void Evaluate_MultipleDenyRules_ReturnsFirstDeny() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Deny,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "first" } }
                    },
                    new EffectRule {
                        Effect = PermissionActions.Deny,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "second" } }
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
            Assert.AreEqual("first", result.Obligations[0].Type);
        }
    }
}
