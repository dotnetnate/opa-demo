using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {
    public class ResourceValidator : AbstractValidator<Resource> {
        public ResourceValidator() {            
            RuleFor(r=>r.Authority).NotEmpty().WithMessage("Authority for the resource's identity is required");
            RuleFor(r => r.Identifier).NotEmpty().WithMessage("Id of the resource is required");            
        }
    }
}
