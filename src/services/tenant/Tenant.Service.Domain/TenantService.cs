using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CitizensFinancialGroup.Elements.CQRS.Commands;
using CitizensFinancialGroup.Elements.CQRS.Queries;

using CitizensFinancialGroup.Threvw.Common;
using CitizensFinancialGroup.Threvw.Common.Identity;
using CitizensFinancialGroup.Threvw.Common.Validation;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public class TenantService<TIdentityContext> : ServiceBase<ClaimsIdentity, TIdentityContext>, ITenantService {
        private readonly ITenantRepository _tenantRepository;

        private const int MAX_PAGE_SIZE = 50;
        private const int DEFAULT_PAGE_SIZE = 10;

        public TenantService(ITenantRepository tenantRepository, IValidationService validationService, IIdentityService<ClaimsIdentity, TIdentityContext> identityService, ILogger<TenantService<TIdentityContext>> logger)
            : base(validationService, logger, identityService) {
            _tenantRepository = tenantRepository;
        }

        // Tenant CRUD operations
        public async Task<QueryResult<IEnumerable<Tenant>>> GetTenants(FindTenantsQuery query) {
            return await ExecuteQuery(query, async (query) => {
                var results = await _tenantRepository.FindTenants(query.PageNumber, Math.Min(query.PageSize, MAX_PAGE_SIZE));                
                return QueryResult<IEnumerable<Tenant>>.SuccessResult(results);                
            });
        }

        public async Task<QueryResult<Tenant>> GetTenantById(FindTenantByIdQuery query) {

            return await ExecuteQuery(query, async (query) => {
                var tenant = await _tenantRepository.FindTenantById(query.Id);
                if (tenant != null) {
                    return QueryResult<Tenant>.SuccessResult(tenant);
                }
                else {
                    return QueryResult<Tenant>.NotFoundResult(tenant);
                }
            });
        }

        public async Task<CommandResult<Tenant>> CreateTenant(CreateTenantCommand command) => await ExecuteCommand(command, async (command) => {
            var tenant = new Tenant {
                Name = command.Name,
                Description = command.Description,
                Id = Guid.NewGuid(),
                Metrics = new UsageMetrics(),
                Settings = new TenantSettings()
            };

            try {
                await _tenantRepository.CreateTenant(tenant);
                return CommandResult.SuccessResult(tenant);
            }
            catch (InvalidOperationException ex) {
                Logger.LogError(ex, "Error creating tenant: {Tenant}", tenant);
                return CommandResult.ResourceConflictResult<Tenant>(validationResult: new InternalValidationResult() { Errors = new List<InternalValidationError> { new InternalValidationError { PropertyName = "tenant.name", ErrorMessage = "The combination of SysId and Name is already in use." } } });
            }
        });

        public async Task<CommandResult<Tenant>> UpdateTenant(UpdateTenantCommand command) => await ExecuteCommand(command, async (command) => {
            var tenant = new Tenant {
                Name = command.Name,
                Description = command.Description,
                Id = command.Id,
                Metrics = new UsageMetrics(),
                Settings = new TenantSettings()
            };

            try {
                await _tenantRepository.UpdateTenant(tenant);
                return CommandResult.SuccessResult(tenant);
            }
            catch (InvalidOperationException ex) {
                Logger.LogError(ex, "Error creating tenant: {Tenant}", tenant);
                return CommandResult.ResourceConflictResult<Tenant>(validationResult: new InternalValidationResult() { Errors = new List<InternalValidationError> { new InternalValidationError { PropertyName = "tenant.name", ErrorMessage = "The combination of SysId and Name is already in use." } } });
            }
        });

        public async Task<CommandResult> DeleteTenant(DeleteTenantCommand command) => await ExecuteCommand(command, async (command) => {
            await _tenantRepository.DeleteTenant(command.Id);
            return CommandResult.SuccessResult();
        });


        // Settings CRUD operations
        public async Task<QueryResult<object>> GetSettingByKey(FindTenantSettingsQuery query) {

            return await ExecuteQuery(query, async (query) => {
                var settings = await _tenantRepository.GetSettingByKey(query.TenantId, query.SettingName);                
                return QueryResult<object>.SuccessResult(settings);                
            });
        }

        public async Task<CommandResult<object>> UpdateSetting(UpdateOptionsGroupCommand command) => await ExecuteCommand(command, async (command) => {
            var settingJson = JsonSerializer.Serialize(command.NewValue, options: new JsonSerializerOptions {  PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await _tenantRepository.UpdateSettingByKey(command.TenantId, command.OptionsGroupName, settingJson);
            return CommandResult.SuccessResult(command.NewValue);
        });

        
    }
}