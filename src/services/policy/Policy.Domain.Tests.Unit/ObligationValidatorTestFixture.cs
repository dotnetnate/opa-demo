using CitizensFinancialGroup.Threvw.Policies.Domain;
using FluentValidation.TestHelper;

namespace Policy.Domain.Tests.Unit {
    [TestClass]
    public class ObligationValidatorTestFixture {
        private ObligationValidator _validator = null!;

        [TestInitialize]
        public void Setup() {
            _validator = new ObligationValidator();
        }

        [TestMethod]
        public void Validate_ValidObligation_NoErrors() {
            // Arrange
            var obligation = new Obligation {
                Type = "approval",
                Parameters = new Dictionary<string, object> {
                    { "approvalType", "manager" },
                    { "minApprovers", 2 }
                }
            };

            // Act
            var result = _validator.TestValidate(obligation);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_EmptyType_HasError() {
            // Arrange
            var obligation = new Obligation {
                Type = "",
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var result = _validator.TestValidate(obligation);

            // Assert
            result.ShouldHaveValidationErrorFor(o => o.Type);
        }

        [TestMethod]
        public void Validate_NullType_HasError() {
            // Arrange
            var obligation = new Obligation {
                Type = null!,
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var result = _validator.TestValidate(obligation);

            // Assert
            result.ShouldHaveValidationErrorFor(o => o.Type);
        }

        [TestMethod]
        public void Validate_WhitespaceType_HasError() {
            // Arrange
            var obligation = new Obligation {
                Type = "   ",
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var result = _validator.TestValidate(obligation);

            // Assert
            result.ShouldHaveValidationErrorFor(o => o.Type);
        }

        [TestMethod]
        public void Validate_EmptyParameters_NoErrors() {
            // Arrange
            var obligation = new Obligation {
                Type = "logging",
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var result = _validator.TestValidate(obligation);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_ComplexParameters_NoErrors() {
            // Arrange
            var obligation = new Obligation {
                Type = "mfa",
                Parameters = new Dictionary<string, object> {
                    { "methods", new[] { "sms", "email", "totp" } },
                    { "timeout", 300 },
                    { "required", true }
                }
            };

            // Act
            var result = _validator.TestValidate(obligation);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
