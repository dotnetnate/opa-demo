using AutoMapper;
using AutoMapper.Features;
using CitizensFinancialGroup.Threvw.Common.Validation;
using CitizensFinancialGroup.Threvw.Policies.Domain;
using CitizensFinancialGroup.Threvw.Tenants.Domain;


namespace CitizensFinancialGroup.Threvw.Policy.Service.Http.TBD {


    public class MappingProfile : Profile {
        public MappingProfile() {
            // Domain-to-Model and Model-to-Domain Mappings
            CreateMap<global::CitizensFinancialGroup.Threvw.Policies.Domain.Policy, Features.Policies.Models.PolicyModel>().ReverseMap();
            CreateMap<Resource, Features.Policies.Models.ResourceModel>().ReverseMap();
            CreateMap<Rule, Features.Policies.Models.RuleModel>().ReverseMap();
            CreateMap<Subject, Features.Policies.Models.SubjectModel>().ReverseMap();
            CreateMap<Privilege, Features.Policies.Models.PrivilegeModel>().ReverseMap();
            CreateMap<Condition, Features.Policies.Models.ConditionModel>().ReverseMap();

            // Request-to-Domain Mappings
            CreateMap<Features.Policies.Models.CreatePolicyRequest, CreatePolicyCommand>();
            CreateMap<Features.Policies.Models.UpdatePolicyRequest, UpdatePolicyCommand>();
            CreateMap<Features.Policies.Models.AddOrUpdateRuleRequest, AddOrUpdateRuleCommand>();
            CreateMap<Features.Policies.Models.FindPoliciesRequest, FindPoliciesQuery>();
            CreateMap<Features.Policies.Models.FindPolicyByResourceRequest, FindPolicyByResourceQuery>();
            CreateMap<Features.Policies.Models.DeleteRuleRequest, DeleteRuleCommand>();

            // Validation Mappings
            CreateMap<InternalValidationError, Features.Shared.Models.InternalValidationError>();
            CreateMap<InternalValidationResult, Features.Shared.Models.InternalValidationResult>();
        }
    }


}
