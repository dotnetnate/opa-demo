namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public class Tenant {
        public required Guid Id { get; set; }
        public required SystemName Name { get; set; }
        public string Description { get; set; } = string.Empty;        
        public required TenantSettings Settings { get; set; } = new TenantSettings();
        public required UsageMetrics Metrics { get; set; } = new UsageMetrics();
    }
}