using FluentValidation;

namespace NOCO.Threvw.Policies.Domain {
    /// <summary>
    /// Validator for the <see cref="Advice"/> class.
    /// </summary>
    public class AdviceValidator : AbstractValidator<Advice> {
        public AdviceValidator() {
            RuleFor(a => a.Type)
                .NotEmpty()
                .WithMessage("Advice type is required.");
        }
    }
}
