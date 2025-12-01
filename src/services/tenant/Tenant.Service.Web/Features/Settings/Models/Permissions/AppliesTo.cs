namespace NOCO.Threvw.Tenants.Service.Http.Features.Settings.Models.Permissions
{

    public class AppliesTo {
        public required ICollection<string> ResourceType { get; set; }
    }
}