using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {

    public class SubjectValidator : AbstractValidator<Subject> {
        public SubjectValidator() {
            RuleFor(s => s.Identifier).NotEmpty();
            RuleFor(s => s.Authority).NotEmpty().Must(authority => Uri.IsWellFormedUriString(authority, UriKind.Absolute))
                .WithMessage("Authority must be a valid URI.");
        }
    }
}
