namespace NOCO.Threvw.Tenants.Service.Http.Features.Tenants.Models {
    public class UpdateTenantRequest {   
        public required SystemName Name { get; set; }
        public string Description { get; set; } = string.Empty;
   
    }
}
