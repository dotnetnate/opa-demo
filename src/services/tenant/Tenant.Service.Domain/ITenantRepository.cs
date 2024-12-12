using CitizensFinancialGroup.Threvw.Tenants.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public interface ITenantRepository {
        Task<IEnumerable<Tenant>> FindTenants(int pageNumber = 0, int pageSize = 0);
        Task<Tenant> FindTenantById(Guid id);
        Task CreateTenant(Tenant tenant);
        Task UpdateTenant(Tenant tenant);
        Task DeleteTenant(Guid id);
        Task<TenantSettings?> GetSettings(Guid tenantId);
        Task<object?> GetSettingByKey(Guid tenantId, string key);
        Task UpdateSettings(Guid tenantId, TenantSettings settings);
        Task UpdateSettingByKey(Guid tenantId, string key, object value);
    }
}
