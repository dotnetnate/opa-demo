using NOCO.Threvw.Policies.Domain;
using FluentValidation.TestHelper;

namespace Policy.Domain.Tests.Unit {
    [TestClass]
    public class EffectRuleValidatorTestFixture {
        private EffectRuleValidator _validator = null!;

        [TestInitialize]
        public void Setup() {
            _validator = new EffectRuleValidator();
        }

        [TestMethod]
        public void Validate_ValidEffectRule_NoErrors() {
            // Arrange
            var effectRule = new EffectRule {
                Effect = PermissionActions.Permit,
                Conditions = new List<Condition>(),
                Obligations = new List<Obligation>(),
                Advice = new List<Advice>()
            };

            // Act
            var result = _validator.TestValidate(effectRule);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_LapsedValidityPeriod_HasError() {
            // Arrange
            var effectRule = new EffectRule {
                Effect = PermissionActions.Permit,
                ValidityPeriod = new NOCO.Elements.Range<DateTimeOffset>(
                    DateTimeOffset.Now.AddDays(-10),
                    DateTimeOffset.Now.AddDays(-1)) {
                    Type = NOCO.Elements.Range<DateTimeOffset>.RangeType.Inclusive
                },
                Conditions = new List<Condition>(),
                Obligations = new List<Obligation>(),
                Advice = new List<Advice>()
            };

            // Act
            var result = _validator.TestValidate(effectRule);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.HasValidityPeriodLapsed());
        }

        [TestMethod]
        public void Validate_ValidValidityPeriod_NoErrors() {
            // Arrange
            var effectRule = new EffectRule {
                Effect = PermissionActions.Permit,
                ValidityPeriod = new NOCO.Elements.Range<DateTimeOffset>(
                    DateTimeOffset.Now,
                    DateTimeOffset.Now.AddDays(30)) {
                    Type = NOCO.Elements.Range<DateTimeOffset>.RangeType.Inclusive
                },
                Conditions = new List<Condition>(),
                Obligations = new List<Obligation>(),
                Advice = new List<Advice>()
            };

            // Act
            var result = _validator.TestValidate(effectRule);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_InvalidCondition_HasError() {
            // Arrange
            var effectRule = new EffectRule {
                Effect = PermissionActions.Permit,
                Conditions = new List<Condition> {
                    new Condition {
                        ContextAttributePath = "", // Invalid
                        Operator = Operators.eq,
                        Value = null
                    }
                },
                Obligations = new List<Obligation>(),
                Advice = new List<Advice>()
            };

            // Act
            var result = _validator.TestValidate(effectRule);

            // Assert
            result.ShouldHaveValidationErrorFor("Conditions[0].ContextAttributePath");
        }

        [TestMethod]
        public void Validate_InvalidObligation_HasError() {
            // Arrange
            var effectRule = new EffectRule {
                Effect = PermissionActions.Permit,
                Conditions = new List<Condition>(),
                Obligations = new List<Obligation> {
                    new Obligation { Type = "" } // Invalid
                },
                Advice = new List<Advice>()
            };

            // Act
            var result = _validator.TestValidate(effectRule);

            // Assert
            result.ShouldHaveValidationErrorFor("Obligations[0].Type");
        }

        [TestMethod]
        public void Validate_InvalidAdvice_HasError() {
            // Arrange
            var effectRule = new EffectRule {
                Effect = PermissionActions.Permit,
                Conditions = new List<Condition>(),
                Obligations = new List<Obligation>(),
                Advice = new List<Advice> {
                    new Advice { Type = "" } // Invalid
                }
            };

            // Act
            var result = _validator.TestValidate(effectRule);

            // Assert
            result.ShouldHaveValidationErrorFor("Advice[0].Type");
        }
    }
}
