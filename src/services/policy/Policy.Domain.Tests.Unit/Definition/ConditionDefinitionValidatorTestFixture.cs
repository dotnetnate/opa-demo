using NOCO.Threvw.Policies.Definition;
using FluentValidation.Results;

namespace Policy.Domain.Tests.Unit;

[TestClass]
public class ConditionDefinitionValidatorTestFixture {

    private ConditionDefinitionValidator _validator;

    [TestInitialize]
    public void Setup() {
        _validator = new ConditionDefinitionValidator();
    }

    [TestMethod]
    public void Given_ValidConditionDefinition_When_Validated_Then_ShouldPassValidation() {
        // Arrange
        var conditionDefinition = new ConditionDefinition {
            InputSource = InputSources.TransactionContext,
            ContextAttributePath = "path.to.attribute",
            DataType = "string",
            Required = true
        };

        // Act
        ValidationResult result = _validator.Validate(conditionDefinition);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Given_InvalidInputSource_When_Validated_Then_ShouldFailValidation() {
        // Arrange
        var conditionDefinition = new ConditionDefinition {
            InputSource = (InputSources)999, // Invalid enum value
            ContextAttributePath = "path.to.attribute",
            DataType = "string",
            Required = true
        };

        // Act
        ValidationResult result = _validator.Validate(conditionDefinition);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage == "Input source is required and must be a valid data source."));
    }

    [TestMethod]
    public void Given_EmptyAttributePath_When_Validated_Then_ShouldFailValidation() {
        // Arrange
        var conditionDefinition = new ConditionDefinition {
            InputSource = InputSources.TransactionContext,
            ContextAttributePath = string.Empty,
            DataType = "string",
            Required = true
        };

        // Act
        ValidationResult result = _validator.Validate(conditionDefinition);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage == "Attribute path is required."));
    }

    [TestMethod]
    public void Given_EmptyDataType_When_Validated_Then_ShouldFailValidation() {
        // Arrange
        var conditionDefinition = new ConditionDefinition {
            InputSource = InputSources.TransactionContext,
            ContextAttributePath = "path.to.attribute",
            DataType = string.Empty,
            Required = true
        };

        // Act
        ValidationResult result = _validator.Validate(conditionDefinition);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage == "Data type is required."));
    }

    [TestMethod]
    public void Given_AttributeName_That_Starts_With_Period_When_Validated_Then_Should_Fail_Validation() {
        // Arrange
        var conditionDefinition = new ConditionDefinition {
            InputSource = InputSources.TransactionContext,
            ContextAttributePath = ".myattr",
            DataType = string.Empty,
            Required = true
        };

        // Act
        ValidationResult result = _validator.Validate(conditionDefinition);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage == "Attribute path must only contain letters and periods and cannot start or end with a period."));
    }

    [TestMethod]
    public void Given_AttributeName_That_Ends_With_Period_When_Validated_Then_Should_Fail_Validation() {
        // Arrange
        var conditionDefinition = new ConditionDefinition {
            InputSource = InputSources.TransactionContext,
            ContextAttributePath = "myattr.",
            DataType = string.Empty,
            Required = true
        };

        // Act
        ValidationResult result = _validator.Validate(conditionDefinition);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage == "Attribute path must only contain letters and periods and cannot start or end with a period."));
    }

    [TestMethod]
    public void Given_AttributeName_That_Contains_Number_When_Validated_Then_Should_Fail_Validation() {
        // Arrange
        var conditionDefinition = new ConditionDefinition {
            InputSource = InputSources.TransactionContext,
            ContextAttributePath = "my1attr",
            DataType = string.Empty,
            Required = true
        };

        // Act
        ValidationResult result = _validator.Validate(conditionDefinition);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage == "Attribute path must only contain letters and periods and cannot start or end with a period."));
    }

    [TestMethod]
    public void Given_UnsupportedDataType_When_Validated_Then_ShouldFailValidation() {
        // Arrange
        var conditionDefinition = new ConditionDefinition {
            InputSource = InputSources.TransactionContext,
            ContextAttributePath = "path.to.attribute",
            DataType = "unsupportedType",
            Required = true
        };

        // Act
        ValidationResult result = _validator.Validate(conditionDefinition);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage == "Data type is not supported."));
    }
}

