using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    public class PrivilegeValidator : AbstractValidator<Privilege> {
        public PrivilegeValidator() {
            RuleFor(p => p.PermissionName).NotEmpty();
            RuleFor(p=> p.HasValidityPeriodLapsed()).Equal(false).WithMessage("The validity period has lapsed.");
            RuleForEach(p => p.Conditions).SetValidator(new ConditionValidator());
        }
    }
}
