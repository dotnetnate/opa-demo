using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Validator for the <see cref="Rule"/> class.
    /// </summary>
    public class RuleValidator : AbstractValidator<Rule> {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleValidator"/> class.
        /// </summary>
        public RuleValidator() {
            RuleFor(r=>r.Subject).NotNull().WithMessage("Subject is required.");
            RuleFor(r => r.Privileges).NotNull().WithMessage("Privileges are required.");
            RuleFor(r => r.Subject).SetValidator(new SubjectValidator());
            RuleForEach(r => r.Privileges).SetValidator(new PrivilegeValidator());
        }
    }
}
