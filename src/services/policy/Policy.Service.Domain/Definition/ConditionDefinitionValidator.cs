using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Definition {

    public class ConditionDefinitionValidator : AbstractValidator<ConditionDefinition> {
        public ConditionDefinitionValidator() {
            RuleFor(x => x.InputSource).IsInEnum().WithMessage("Input source is required and must be a valid data source.");
            RuleFor(x => x.ContextAttributePath).NotEmpty().WithMessage("Attribute path is required.");            
            RuleFor(x => x.ContextAttributePath).Matches(@"^[a-zA-Z]+(\.[a-zA-Z]+)*$").WithMessage("Attribute path must only contain letters and periods and cannot start or end with a period.");
            RuleFor(x => x.DataType).NotEmpty().WithMessage("Data type is required.");
            RuleFor(x => x.DataType).Custom((dataType, context) => {
                if (!TypeOperandOptions.GetSupportedTypes().Contains(dataType)) {
                    context.AddFailure("Data type is not supported.");
                }
            });

        }
    }
}
