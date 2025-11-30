using CitizensFinancialGroup.Threvw.Policies.Domain;
using FluentValidation.TestHelper;

namespace Policy.Domain.Tests.Unit {
    [TestClass]
    public class PrivilegeValidatorTestFixture {
        private PrivilegeValidator _validator = null!;

        [TestInitialize]
        public void Setup() {
            _validator = new PrivilegeValidator();
        }

        [TestMethod]
        public void Validate_ValidPrivilege_NoErrors() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "READ",
                CombiningAlgorithm = CombiningAlgorithm.FirstApplicable,
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation>(),
                        Advice = new List<Advice>()
                    }
                },
                DefaultEffect = PermissionActions.Deny
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_EmptyPermissionName_HasError() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>()
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.PermissionName);
        }

        [TestMethod]
        public void Validate_NullPermissionName_HasError() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = null!,
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>()
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.PermissionName);
        }

        [TestMethod]
        public void Validate_NoEffectRules_HasError() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "WRITE",
                EffectRules = new List<EffectRule>()
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.EffectRules)
                .WithErrorMessage("At least one effect rule is required.");
        }

        [TestMethod]
        public void Validate_NullEffectRules_HasError() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "DELETE",
                EffectRules = null!
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.EffectRules);
        }

        [TestMethod]
        public void Validate_InvalidEffectRule_HasError() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "WRITE",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        ValidityPeriod = new CitizensFinancialGroup.Elements.Range<DateTimeOffset>(
                            DateTimeOffset.Now.AddDays(-10),
                            DateTimeOffset.Now.AddDays(-1)) { // Expired
                            Type = CitizensFinancialGroup.Elements.Range<DateTimeOffset>.RangeType.Inclusive
                        },
                        Conditions = new List<Condition>()
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldHaveValidationErrorFor("EffectRules[0]");
        }

        [TestMethod]
        public void Validate_MultipleValidEffectRules_NoErrors() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "WIRE_TRANSFER",
                CombiningAlgorithm = CombiningAlgorithm.FirstApplicable,
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition> {
                            new Condition {
                                ContextAttributePath = "amount",
                                Operator = Operators.lt,
                                Value = 10000
                            }
                        },
                        Obligations = new List<Obligation> {
                            new Obligation { Type = "logging" }
                        }
                    },
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition> {
                            new Condition {
                                ContextAttributePath = "amount",
                                Operator = Operators.gte,
                                Value = 10000
                            }
                        },
                        Obligations = new List<Obligation> {
                            new Obligation { Type = "approval" }
                        },
                        Advice = new List<Advice> {
                            new Advice { Type = "monitor" }
                        }
                    }
                },
                DefaultEffect = PermissionActions.Deny
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_EffectRuleWithInvalidCondition_HasError() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "READ",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition> {
                            new Condition {
                                ContextAttributePath = "", // Invalid
                                Operator = Operators.eq,
                                Value = null
                            }
                        }
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldHaveValidationErrorFor("EffectRules[0].Conditions[0].ContextAttributePath");
        }

        [TestMethod]
        public void Validate_EffectRuleWithInvalidObligation_HasError() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "WRITE",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Permit,
                        Conditions = new List<Condition>(),
                        Obligations = new List<Obligation> {
                            new Obligation { Type = "" } // Invalid
                        }
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldHaveValidationErrorFor("EffectRules[0].Obligations[0].Type");
        }

        [TestMethod]
        public void Validate_EffectRuleWithInvalidAdvice_HasError() {
            // Arrange
            var privilege = new Privilege {
                PermissionName = "DELETE",
                EffectRules = new List<EffectRule> {
                    new EffectRule {
                        Effect = PermissionActions.Deny,
                        Conditions = new List<Condition>(),
                        Advice = new List<Advice> {
                            new Advice { Type = "" } // Invalid
                        }
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(privilege);

            // Assert
            result.ShouldHaveValidationErrorFor("EffectRules[0].Advice[0].Type");
        }

        [TestMethod]
        public void Validate_AllCombiningAlgorithms_NoErrors() {
            // Test all combining algorithms are valid
            var algorithms = new[] {
                CombiningAlgorithm.FirstApplicable,
                CombiningAlgorithm.DenyOverrides,
                CombiningAlgorithm.PermitOverrides,
                CombiningAlgorithm.OnlyOneApplicable
            };

            foreach (var algorithm in algorithms) {
                var privilege = new Privilege {
                    PermissionName = $"TEST_{algorithm}",
                    CombiningAlgorithm = algorithm,
                    EffectRules = new List<EffectRule> {
                        new EffectRule {
                            Effect = PermissionActions.Permit,
                            Conditions = new List<Condition>()
                        }
                    },
                    DefaultEffect = PermissionActions.Deny
                };

                var result = _validator.TestValidate(privilege);
                result.ShouldNotHaveAnyValidationErrors();
            }
        }
    }
}
