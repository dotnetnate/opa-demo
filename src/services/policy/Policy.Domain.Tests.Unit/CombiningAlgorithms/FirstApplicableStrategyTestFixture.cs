using NOCO.Threvw.Policies.Domain;
using NOCO.Threvw.Policies.Domain.CombiningAlgorithms;
using Moq;

namespace Policy.Domain.Tests.Unit.CombiningAlgorithms {
    [TestClass]
    public class FirstApplicableStrategyTestFixture {
        private FirstApplicableStrategy _strategy = null!;
        private Mock<IConditionEvaluator> _mockEvaluator = null!;

        [TestInitialize]
        public void Setup() {
            _strategy = new FirstApplicableStrategy();
            _mockEvaluator = new Mock<IConditionEvaluator>();
        }

        [TestMethod]
        public void Algorithm_ReturnsFirstApplicable() {
            // Assert
            Assert.AreEqual(CombiningAlgorithm.FirstApplicable, _strategy.Algorithm);
        }

        [TestMethod]
        public void Evaluate_NoEffectRules_ReturnsDefaultEffect() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule>(),
                DefaultEffect = PermissionActions.Deny
            };
            var context = new Dictionary<string, object>();

            // Act
            var result = _strategy.Evaluate(privilege, context, _mockEvaluator.Object);

            // Assert
            Assert.AreEqual(PolicyDecision.Deny, result.Decision);
            Assert.IsTrue(result.EvaluationDetails!.Contains("No rules matched"));
        }

        [TestMethod]
        public void Evaluate_FirstRuleMatches_ReturnsFirstRule() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> {
                            new Obligation { Type = "logging" }
                        }
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
            Assert.AreEqual(PolicyDecision.Permit, result.Decision);
            Assert.AreEqual(1, result.Obligations.Count);
            Assert.AreEqual("logging", result.Obligations[0].Type);
        }

        [TestMethod]
        public void Evaluate_FirstRuleDoesNotMatch_SecondRuleMatches_ReturnsSecondRule() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition> {
                            new Condition { ContextAttributePath = "amount", Operator = Operators.gt, Value = 1000 }
                        }
                    },
                    new EffectRule {
                        Effect = PermissionActions.Deny,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> {
                            new Obligation { Type = "notification" }
                        }
                    }
                },
                DefaultEffect = PermissionActions.Deny
            };
            var context = new Dictionary<string, object>();

            // First rule fails, second rule passes
            _mockEvaluator.SetupSequence(e => e.EvaluateConditions(It.IsAny<List<Condition>>(), context))
                .Returns(false)
                .Returns(true);

            // Act
            var result = _strategy.Evaluate(privilege, context, _mockEvaluator.Object);

            // Assert
            Assert.AreEqual(PolicyDecision.Deny, result.Decision);
            Assert.AreEqual(1, result.Obligations.Count);
            Assert.AreEqual("notification", result.Obligations[0].Type);
        }

        [TestMethod]
        public void Evaluate_ValidityPeriodLapsed_SkipsRule() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        ValidityPeriod = new NOCO.Elements.Range<DateTimeOffset>(
                            DateTimeOffset.Now.AddDays(-10),
                            DateTimeOffset.Now.AddDays(-1)) { // Expired
                            Type = NOCO.Elements.Range<DateTimeOffset>.RangeType.Inclusive
                        },
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
            Assert.AreEqual(PolicyDecision.Deny, result.Decision);
            Assert.IsTrue(result.EvaluationDetails!.Contains("validity period lapsed"));
        }

        [TestMethod]
        public void Evaluate_ReturnsAdviceFromMatchingRule() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "TEST",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Advice = new List<Advice> {
                            new Advice { Type = "monitor", Parameters = new Dictionary<string, object> { { "level", "high" } } }
                        }
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
            Assert.AreEqual(1, result.Advice.Count);
            Assert.AreEqual("monitor", result.Advice[0].Type);
        }
    }
}
