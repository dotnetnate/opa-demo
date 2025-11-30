using FluentValidation;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Validator for the <see cref="Obligation"/> class.
    /// </summary>
    public class ObligationValidator : AbstractValidator<Obligation> {
        public ObligationValidator() {
            RuleFor(o => o.Type)
                .NotEmpty()
                .WithMessage("Obligation type is required.");
        }
    }
}
