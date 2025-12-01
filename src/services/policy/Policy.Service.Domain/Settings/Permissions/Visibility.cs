namespace NOCO.Threvw.Tenants.Domain {
    public class Visibility {
        public required bool Enabled { get; set; }
        public DateTimeOffset? ActivationDate { get; set; }
        public DateTimeOffset? DeactivationDate { get; set; }
    }
}