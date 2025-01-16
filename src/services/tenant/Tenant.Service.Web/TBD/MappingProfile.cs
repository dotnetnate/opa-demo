using AutoMapper;
using CitizensFinancialGroup.Elements.Validation;
using CitizensFinancialGroup.Threvw.Tenants.Domain;
using CitizensFinancialGroup.Threvw.Tenants.Service.Http.Features.Settings.Models;
using CitizensFinancialGroup.Threvw.Tenants.Service.Http.Features.Tenants.Models;

namespace CitizensFinancialGroup.Threvw.Tenants.Service.Http.TBD {


    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<Domain.Tenant, Features.Tenants.Models.Tenant>().ReverseMap();
            CreateMap<Domain.SystemName, Features.Tenants.Models.SystemName>().ReverseMap();
            CreateMap<Domain.PermissionDefinition, Features.Settings.Models.Permissions.PermissionDefinition>().ReverseMap();
            CreateMap<Domain.PermissionOptionsGroup, Features.Settings.Models.Permissions.PermissionOptionsGroup>().ReverseMap();
            CreateMap<Domain.Visibility, Features.Settings.Models.Permissions.Visibility>().ReverseMap();
            CreateMap<Domain.ConditionDefinition, Features.Settings.Models.Permissions.ConditionDefinition>().ReverseMap();
            CreateMap<Domain.AppliesTo, Features.Settings.Models.Permissions.AppliesTo>().ReverseMap();
            CreateMap<Domain.Metric, Features.Metrics.Models.Metric>().ReverseMap();
            CreateMap<Domain.UsageMetrics, Features.Metrics.Models.UsageMetrics>().ReverseMap();
            CreateMap<Domain.MetricSample, Features.Metrics.Models.MetricSample>().ReverseMap();
            CreateMap<Features.Tenants.Models.CreateTenantRequest, Domain.CreateTenantCommand>();
            CreateMap<Features.Tenants.Models.UpdateTenantRequest, Domain.UpdateTenantCommand>();
            CreateMap<Features.Tenants.Models.FindTenantsRequest, Domain.FindTenantsQuery>();
            CreateMap<Features.Tenants.Models.FindTenantByIdRequest, Domain.FindTenantByIdQuery>();
            CreateMap<Features.Tenants.Models.DeleteTenantRequest, Domain.DeleteTenantCommand>();
            CreateMap<InternalValidationError, Features.Shared.Models.InternalValidationError>();
            CreateMap<InternalValidationResult, Features.Shared.Models.InternalValidationResult>();
            CreateMap<UpdateTenantRequest, UpdateTenantCommand>();
            CreateMap<GetTenantSettingsRequest, FindTenantSettingsQuery>();
            CreateMap<UpdateOptionsGroupRequest, UpdateOptionsGroupCommand>();
        }
    }


}
