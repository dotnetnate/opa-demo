namespace CitizensFinancialGroup.Threvw.Tenants.Service.Http.Features.Settings.Models.Permissions
{
    public class Visibility {
        public required bool Enabled { get; set; }
        public DateTimeOffset? ActivationDate { get; set; }
        public DateTimeOffset? DeactivationDate { get; set; }
    }
}