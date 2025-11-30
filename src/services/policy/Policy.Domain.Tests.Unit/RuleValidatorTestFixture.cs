using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentValidation.TestHelper;
using CitizensFinancialGroup.Threvw.Policies.Domain;

namespace CitizensFinancialGroup.Threvw.Policies.Domain.Tests {
    [TestClass]
    public class RuleValidatorTestFixture {
        private RuleValidator _validator;

        [TestInitialize]
        public void Setup() {
            _validator = new RuleValidator();
        }

        [TestMethod]
        public void Given_Valid_Rule_When_Validated_Then_No_Validation_Errors() {
            // Arrange
            var rule = new Rule {
                Subject = new Subject { Authority = "https://someuri.org/", Identifier = "SomeId" },
                Privileges = new List<Privilege> {
                    new Privilege {
                        PermissionName = "PERMISSION_1",
                        EffectRules = new List<EffectRule> {
                            new EffectRule {
                                Effect = PermissionActions.Permit,
                                Conditions = new List<Condition>()
                            }
                        }
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(rule);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Given_Rule_With_Null_Subject_When_Validated_Then_Validation_Error_For_Subject() {
            // Arrange
            var rule = new Rule {
                Subject = null,
                Privileges = new List<Privilege> {
                    new Privilege {
                        PermissionName = "PERMISSION_1",
                        EffectRules = new List<EffectRule> {
                            new EffectRule {
                                Effect = PermissionActions.Permit,
                                Conditions = new List<Condition>()
                            }
                        }
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(rule);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Subject);
        }

        [TestMethod]
        public void Given_Rule_With_Invalid_Privilege_When_Validated_Then_Validation_Error_For_Privilege() {
            // Arrange
            var rule = new Rule {
                Subject = new Subject { Authority = "https://someuri.org/", Identifier = "SomeId" },
                Privileges = new List<Privilege> {
                    new Privilege { PermissionName = null }
                }
            };

            // Act
            var result = _validator.TestValidate(rule);

            // Assert
            result.ShouldHaveValidationErrorFor("Privileges[0].PermissionName");
        }
    }
}
