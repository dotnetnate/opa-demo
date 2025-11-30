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
            
            RuleFor(p => p.EffectRules)
                .NotEmpty()
                .WithMessage("At least one effect rule is required.");
                
            RuleForEach(p => p.EffectRules)
                .SetValidator(new EffectRuleValidator());
        }
    }
}
