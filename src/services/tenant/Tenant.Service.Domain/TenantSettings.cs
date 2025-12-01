using NOCO.Threvw.Tenants.Domain;
using Microsoft.Extensions.Options;

namespace NOCO.Threvw.Tenants.Domain {
    public class TenantSettings {
        public PermissionOptionsGroup Permissions { get; set; } = new();
    }
}