using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitizensFinancialGroup.Threvw.Policies.Domain.Tests {
    [TestClass]
    public class ResourceValidatorTests {
        private ResourceValidator _validator;

        [TestInitialize]
        public void Setup() {
            _validator = new ResourceValidator();
        }

        [TestMethod]
        public void Given_Resource_With_Empty_Authority_When_Validated_Then_Should_Have_Validation_Error() {
            // Arrange
            var resource = new Resource { Authority = string.Empty, Identifier = "123" };

            // Act
            var result = _validator.TestValidate(resource);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Authority);
        }

        [TestMethod]
        public void Given_Resource_With_Empty_Id_When_Validated_Then_Should_Have_Validation_Error() {
            // Arrange
            var resource = new Resource { Authority = "https://someuri.org/", Identifier = string.Empty };

            // Act
            var result = _validator.TestValidate(resource);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Identifier);
        }



        [TestMethod]
        public void Given_Resource_With_Valid_Properties_When_Validated_Then_Should_Not_Have_Validation_Errors() {
            // Arrange
            var resource = new Resource { Authority = "https://someuri.org/", Identifier = "123"};

            // Act
            var result = _validator.TestValidate(resource);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
