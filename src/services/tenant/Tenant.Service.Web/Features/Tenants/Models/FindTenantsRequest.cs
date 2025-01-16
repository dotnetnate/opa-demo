namespace CitizensFinancialGroup.Threvw.Tenants.Service.Http.Features.Tenants.Models {
    public class FindTenantsRequest {
        public string? NameFilter { get; set; }
        public string? IdFilter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
