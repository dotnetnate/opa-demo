using NOCO.Threvw.Policies.Domain;
using FluentValidation.TestHelper;

namespace Policy.Domain.Tests.Unit {
    [TestClass]
    public class AdviceValidatorTestFixture {
        private AdviceValidator _validator = null!;

        [TestInitialize]
        public void Setup() {
            _validator = new AdviceValidator();
        }

        [TestMethod]
        public void Validate_ValidAdvice_NoErrors() {
            // Arrange
            var advice = new Advice {
                Type = "monitor",
                Parameters = new Dictionary<string, object> {
                    { "level", "high" },
                    { "flagForReview", true }
                }
            };

            // Act
            var result = _validator.TestValidate(advice);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_EmptyType_HasError() {
            // Arrange
            var advice = new Advice {
                Type = "",
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var result = _validator.TestValidate(advice);

            // Assert
            result.ShouldHaveValidationErrorFor(a => a.Type);
        }

        [TestMethod]
        public void Validate_NullType_HasError() {
            // Arrange
            var advice = new Advice {
                Type = null!,
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var result = _validator.TestValidate(advice);

            // Assert
            result.ShouldHaveValidationErrorFor(a => a.Type);
        }

        [TestMethod]
        public void Validate_WhitespaceType_HasError() {
            // Arrange
            var advice = new Advice {
                Type = "   ",
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var result = _validator.TestValidate(advice);

            // Assert
            result.ShouldHaveValidationErrorFor(a => a.Type);
        }

        [TestMethod]
        public void Validate_EmptyParameters_NoErrors() {
            // Arrange
            var advice = new Advice {
                Type = "audit",
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var result = _validator.TestValidate(advice);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_ComplexParameters_NoErrors() {
            // Arrange
            var advice = new Advice {
                Type = "rateLimit",
                Parameters = new Dictionary<string, object> {
                    { "maxRequests", 100 },
                    { "windowSeconds", 60 },
                    { "alertThreshold", 0.8 }
                }
            };

            // Act
            var result = _validator.TestValidate(advice);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
