using FluentValidation;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {


    public class PolicyValidator : AbstractValidator<Policy> {
        public PolicyValidator() {
            RuleFor(p => p.Resource).SetValidator(new ResourceValidator());
            RuleForEach(p => p.Rules).SetValidator(new RuleValidator());
        }
    }

    public class ResourceValidator : AbstractValidator<Resource> {
        public ResourceValidator() {
            RuleFor(r => r.ResourceId).NotEmpty();
            RuleFor(r => r.ResourceType).NotEmpty();
        }
    }

    public class RuleValidator : AbstractValidator<Rule> {
        public RuleValidator() {
            RuleFor(r => r.Subject).SetValidator(new SubjectValidator());
            RuleForEach(r => r.Privileges).SetValidator(new PrivilegeValidator());
        }
    }

    public class SubjectValidator : AbstractValidator<Subject> {
        public SubjectValidator() {
            RuleFor(s => s.Identifier).NotEmpty();
            RuleFor(s => s.Type).NotEmpty().Must(type => type == "user" || type == "role")
                .WithMessage("Subject type must be either 'user' or 'role'.");
            RuleFor(s => s.Authority).NotEmpty().Must(authority => Uri.IsWellFormedUriString(authority, UriKind.Absolute))
                .WithMessage("Authority must be a valid URI.");
        }
    }

    public class PrivilegeValidator : AbstractValidator<Privilege> {
        public PrivilegeValidator() {
            RuleFor(p => p.Name).NotEmpty();
            RuleForEach(p => p.Conditions).SetValidator(new ConditionValidator());
        }
    }

}