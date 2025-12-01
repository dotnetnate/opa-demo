using NOCO.Threvw.Policies.Domain;
using NOCO.Threvw.Policies.Domain.CombiningAlgorithms;
using Moq;

namespace Policy.Domain.Tests.Unit.CombiningAlgorithms {
    [TestClass]
    public class OnlyOneApplicableStrategyTestFixture {
        private OnlyOneApplicableStrategy _strategy = null!;
        private Mock<IConditionEvaluator> _mockEvaluator = null!;

        [TestInitialize]
        public void Setup() {
            _strategy = new OnlyOneApplicableStrategy();
            _mockEvaluator = new Mock<IConditionEvaluator>();
        }

        [TestMethod]
        public void Algorithm_ReturnsOnlyOneApplicable() {
            // Assert
            Assert.AreEqual(CombiningAlgorithm.OnlyOneApplicable, _strategy.Algorithm);
        }

        [TestMethod]
        public void Evaluate_ExactlyOneRuleMatches_ReturnsRuleDecision() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> { new Obligation { Type = "approval" } }
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
            Assert.IsTrue(result.EvaluationDetails!.Contains("Exactly one rule matched"));
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
        public void Evaluate_MultipleRulesMatch_ReturnsIndeterminate() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>()
                    },
                    new EffectRule {
                        Effect = PermissionActions.Deny,
                        Conditions = new List<Condition>()
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
            Assert.AreEqual(PolicyDecision.Indeterminate, result.Decision);
            Assert.IsTrue(result.EvaluationDetails!.Contains("2 rules matched"));
            Assert.IsTrue(result.EvaluationDetails!.Contains("ERROR"));
        }

        [TestMethod]
        public void Evaluate_TwoRulesOneExpired_ReturnsValidRuleDecision() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Deny,
                        ValidityPeriod = new NOCO.Elements.Range<DateTimeOffset>(
                            DateTimeOffset.Now.AddDays(-10),
                            DateTimeOffset.Now.AddDays(-1)) { // Expired
                            Type = NOCO.Elements.Range<DateTimeOffset>.RangeType.Inclusive
                        },
                        Conditions = new List<Condition>()
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
            Assert.IsTrue(result.EvaluationDetails!.Contains("Exactly one rule matched"));
        }
    }
}
