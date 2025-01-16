using FluentValidation;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {


    public class PolicyValidator : AbstractValidator<Policy> {
        public PolicyValidator() {
            RuleFor(p=>p.Resource).NotNull().WithMessage("Resource is required.");
            RuleFor(p => p.Rules).NotNull().WithMessage("Rules are required.");
            RuleFor(p => p.Resource).SetValidator(new ResourceValidator());
            RuleForEach(p => p.Rules).SetValidator(new RuleValidator());
        }
    }








}