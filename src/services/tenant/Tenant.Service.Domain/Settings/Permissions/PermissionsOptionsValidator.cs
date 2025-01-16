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

            //RuleForEach(x => x.PermissionDefinitions)
                //.SetValidator(new PermissionDefinitionValidator());
        }

        private bool HaveUniquePermissionNames(List<PermissionDefinition> permissions) {
            var permissionNames = permissions.Select(p => p.Name.ToLowerInvariant()).ToList();
            return permissionNames.Distinct().Count() == permissionNames.Count;
        }
    }
}
