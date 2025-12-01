namespace NOCO.Threvw.Tenants.Service.Http.Features.Tenants.Models {
    public class CreateTenantRequest {   
        public required SystemName Name { get; set; }
        public string Description { get; set; } = string.Empty;
   
    }
}
