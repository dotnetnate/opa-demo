using FluentValidation;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {


    public class PolicyValidator : AbstractValidator<Policy> {
        public PolicyValidator() {
            RuleFor(p=>p.Resource).NotNull().WithMessage("Resource is required.");
            RuleFor(p => p.Rules).NotNull().WithMessage("Rules are required.");
            RuleFor(p => p.Resource).SetValidator(new ResourceValidator());
            RuleForEach(p => p.Rules).SetValidator(new RuleValidator());
            
            // Enforce unique subjects within a policy
            RuleFor(p => p.Rules)
                .Must(rules => {
                    if (rules == null) return true;
                    var subjects = rules.Select(r => r.Subject).ToList();
                    return subjects.Distinct().Count() == subjects.Count;
                })
                .WithMessage("A policy cannot contain duplicate subjects (same authority and identifier).");
        }
    }








}