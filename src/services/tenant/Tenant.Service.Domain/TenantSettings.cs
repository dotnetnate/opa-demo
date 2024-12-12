using CitizensFinancialGroup.Threvw.Tenants.Domain;
using Microsoft.Extensions.Options;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public class TenantSettings {
        public PermissionOptionsGroup Permissions { get; set; } = new();
    }
}