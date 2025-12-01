namespace NOCO.Threvw.Tenants.Service.Http.Features.Settings.Models {
    public class UpdateOptionsGroupRequest {    
        public Guid TenantId { get; set; }        
        public string? OptionsGroupName { get; set; }
        public OptionsGroup NewValue { get; set; }
    }
}
