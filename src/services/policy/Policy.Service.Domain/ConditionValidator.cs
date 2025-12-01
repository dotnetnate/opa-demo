using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace NOCO.Threvw.Policies.Domain {
    
    public class ConditionValidator : AbstractValidator<Condition> {
        public ConditionValidator() {
            RuleFor(c => c.ContextAttributePath).NotEmpty();
            RuleFor(c => c.Operator).IsInEnum().WithMessage("Invalid operator.");
            RuleFor(c => c.Value).NotNull();
        }
    }
}
