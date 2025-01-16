using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policy.Service.Client {

    public class PolicyValidator : AbstractValidator<Policy> {
        public PolicyValidator() {

            RuleFor(policy => policy.Resource)
                .NotNull().WithMessage("Policy must have a resource.");

            RuleForEach(policy => policy.Rules)
                .SetValidator(new RuleValidator());
        }
    }

    public class RuleValidator : AbstractValidator<Rule> {

        public RuleValidator() {

            RuleFor(rule => rule.Subject)
                .NotNull().WithMessage("Rule must have a subject.");

            RuleFor(rule => rule.Subject).SetValidator(new SubjectValidator());

            RuleForEach(rule => rule.Privileges)
                .SetValidator(new PrivilegeValidator());
        }
    }

    public class SubjectValidator : AbstractValidator<Subject> {
        public SubjectValidator() {
            RuleFor(p => p.Identifier).NotEmpty().WithMessage("Identifier must be provided.");
            RuleFor(p => p.Authority).NotEmpty().WithMessage("Authority must be provided.");
        }
    }

    public class PrivilegeValidator : AbstractValidator<Privilege> {
        public PrivilegeValidator() {
            RuleFor(p => p.PermissionName).NotEmpty();
            RuleFor(p => p.HasValidityPeriodLapsed()).Equal(false).WithMessage("The validity period has lapsed.");
            RuleForEach(p => p.Conditions).SetValidator(new ConditionValidator());
        }
    }

    public class ConditionValidator : AbstractValidator<Condition> {
        public ConditionValidator() {
            RuleFor(c => c.ContextAttributePath).NotEmpty().WithMessage("Context attribute path must be provided.");
            RuleFor(c => c.ContextAttributePath).Matches(@"^[a-zA-Z]+(\.[a-zA-Z]+)*$").WithMessage("Attribute path must only contain letters and periods and cannot start or end with a period.");
            RuleFor(c => c.Operator).IsInEnum().WithMessage("Invalid operator.");
            RuleFor(c => c.Value).NotNull();
        }
    }
}
