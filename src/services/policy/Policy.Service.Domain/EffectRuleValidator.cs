using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NOCO.Threvw.Policies.Domain {
    /// <summary>
    /// Validator for the <see cref="EffectRule"/> class.
    /// </summary>
    public class EffectRuleValidator : AbstractValidator<EffectRule> {
        public EffectRuleValidator() {
            RuleFor(er => er.HasValidityPeriodLapsed())
                .Equal(false)
                .WithMessage("The validity period has lapsed.");
                
            RuleForEach(er => er.Conditions)
                .SetValidator(new ConditionValidator());
                
            RuleForEach(er => er.Obligations)
                .SetValidator(new ObligationValidator());
                
            RuleForEach(er => er.Advice)
                .SetValidator(new AdviceValidator());
        }
    }
}
