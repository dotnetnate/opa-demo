using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain.Settings.Permissions {
    public class PermissionsOptionsValidator : AbstractValidator<PermissionOptionsGroup> {
        public PermissionsOptionsValidator() {
            RuleFor(x => x.PermissionDefinitions)
                .Must(HaveUniquePermissionNames)
                .WithMessage("Permissions must have unique names.");

            RuleForEach(x => x.PermissionDefinitions)
                .SetValidator(new PermissionDefinitionValidator());
        }

        private bool HaveUniquePermissionNames(List<PermissionDefinition> permissions) {
            var permissionNames = permissions.Select(p => p.Name.ToLowerInvariant()).ToList();
            return permissionNames.Distinct().Count() == permissionNames.Count;
        }
    }

    public class PermissionDefinitionValidator : AbstractValidator<PermissionDefinition> {
        public PermissionDefinitionValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Permission name is required.");
            RuleFor(x => x.Visibility).NotEmpty().WithMessage("Visibility is required.");
            RuleForEach(x => x.AllowedConditions).SetValidator(new ConditionDefinitionValidator());
        }
    }

    public class ConditionDefinitionValidator : AbstractValidator<ConditionDefinition> {
        public ConditionDefinitionValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Condition name is required.");
            RuleFor(x => x.DataType).NotEmpty().WithMessage("Data type is required.");
            RuleFor(x=>x.DataType).Custom((dataType, context) => {
                if (TypeOperandOptions.GetSupportedTypes().Contains(dataType)) {
                    context.AddFailure("Data type is not supported.");
                }
            });

        }
    }
}
