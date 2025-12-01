using FluentValidation.TestHelper;


namespace NOCO.Threvw.Policies.Domain.Tests {
    [TestClass]
    public class SubjectValidatorTestFixture {
        private readonly SubjectValidator _validator;

        public SubjectValidatorTestFixture() {
            _validator = new SubjectValidator();
        }

        [TestMethod]
        public void Given_Empty_Id_When_Validated_Then_Should_Have_Error() {
            var subject = new Subject { Identifier = string.Empty, Authority = "http://valid.uri" };
            var result = _validator.TestValidate(subject);
            result.ShouldHaveValidationErrorFor(s => s.Identifier);
        }

        [TestMethod]
        public void Given_NonEmpty_Id_When_Validated_Then_Should_Not_Have_Error() {
            var subject = new Subject { Identifier = "123", Authority = "http://valid.uri" };
            var result = _validator.TestValidate(subject);
            result.ShouldNotHaveValidationErrorFor(s => s.Identifier);
        }

        [TestMethod]
        public void Given_Empty_Authority_When_Validated_Then_Should_Have_Error() {
            var subject = new Subject { Identifier = "123", Authority = string.Empty };
            var result = _validator.TestValidate(subject);
            result.ShouldHaveValidationErrorFor(s => s.Authority);
        }

        [TestMethod]
        public void Given_Invalid_Uri_Authority_When_Validated_Then_Should_Have_Error() {
            var subject = new Subject { Identifier = "123", Authority = "invalid_uri" };
            var result = _validator.TestValidate(subject);
            result.ShouldHaveValidationErrorFor(s => s.Authority)
                .WithErrorMessage("Authority must be a valid URI.");
        }

        [TestMethod]
        public void Given_Valid_Uri_Authority_When_Validated_Then_Should_Not_Have_Error() {
            var subject = new Subject { Identifier = "123", Authority = "http://valid.uri" };
            var result = _validator.TestValidate(subject);
            result.ShouldNotHaveValidationErrorFor(s => s.Authority);
        }
    }
}
