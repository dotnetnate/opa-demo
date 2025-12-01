namespace NOCO.Threvw.Tenants.Domain
{

    public class ConditionDefinition
    {
        public required string Name { get; set; }
        public required string DataType { get; set; }
        public bool Required { get; set; }
    }
}