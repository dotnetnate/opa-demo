using FluentValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;


namespace CitizensFinancialGroup.Threvw.Tenants.Service.Http.Features.Settings.Models.Permissions
{

    public class PermissionOptionsGroup : OptionsGroup{

        public bool EnforceConfiguredPermissions { get; set; }
        public List<PermissionDefinition> PermissionDefinitions { get; set; } = new List<PermissionDefinition>();
    }
}