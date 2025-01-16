using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Definition {
    public class PermissionDefinitionValidator : AbstractValidator<PermissionDefinition> {
        public PermissionDefinitionValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Permission name is required.");
            RuleFor(x => x.Visibility).NotEmpty().WithMessage("Visibility is required.");
            RuleForEach(x => x.AllowedConditions).SetValidator(new ConditionDefinitionValidator());
        }
    }
}
