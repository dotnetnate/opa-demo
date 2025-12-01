namespace NOCO.Threvw.Tenants.Service.Http.Features.Settings.Models.Permissions
{

    public class ConditionDefinition {
        public required string Name { get; set; }
        public required string DataType { get; set; }
        public bool Required { get; set; }
    }
}