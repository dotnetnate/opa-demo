namespace NOCO.Threvw.Tenants.Service.Http.Features.Tenants.Models { 
    public class Tenant {
        public required Guid Id { get; set; }
        public required SystemName Name { get; set; }
        public string Description { get; set; } = string.Empty;                
    }
}