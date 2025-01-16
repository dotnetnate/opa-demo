
using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Elements.ApplicationModel.Queries;
using CitizensFinancialGroup.Elements;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public interface ITenantService {
        Task<CommandResult<Tenant>> CreateTenant(CreateTenantCommand command);
        Task<CommandResult> DeleteTenant(DeleteTenantCommand command);
        Task<QueryResult<object>> GetSettingByKey(FindTenantSettingsQuery query);
        Task<QueryResult<Tenant>> GetTenantById(FindTenantByIdQuery query);
        Task<QueryResult<IEnumerable<Tenant>>> GetTenants(FindTenantsQuery query);
        Task<CommandResult<object>> UpdateSetting(UpdateOptionsGroupCommand command);
        Task<CommandResult<Tenant>> UpdateTenant(UpdateTenantCommand command);
    }
}