using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentValidation.TestHelper;
using NOCO.Threvw.Policies.Domain;

namespace NOCO.Threvw.Policies.Domain.Tests {
    [TestClass]
    public class PolicyValidatorTests {
        [TestMethod]
        public void Given_Valid_Policy_When_Validated_Then_Should_Not_Have_Validation_Errors() {
            // Arrange
            var policy = new Policy {
                Resource = new Resource { Authority = "https://someuri.org", Identifier = "id" },
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        Subject = new Subject {  Authority = "https://someuri.org", Identifier = "id" },
                        Privileges = new List<Privilege>
                        {
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
                    }
                }
            };
            var validator = new PolicyValidator();

            // Act
            var result = validator.TestValidate(policy);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Given_Invalid_Policy_When_Validated_Then_Should_Have_Validation_Errors() {
            // Arrange
            var policy = new Policy {
                Resource = null,
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        Subject = null,
                        Privileges = new List<Privilege>
                        {
                            new Privilege { PermissionName = null }
                        }
                    }
                }
            };
            var validator = new PolicyValidator();

            // Act
            var result = validator.TestValidate(policy);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.Resource);            
            result.ShouldHaveValidationErrorFor("Rules[0].Privileges[0].PermissionName");
        }
    }
}
