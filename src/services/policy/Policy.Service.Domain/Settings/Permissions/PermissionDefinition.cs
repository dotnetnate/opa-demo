namespace CitizensFinancialGroup.Threvw.Tenants.Domain
{

    public class PermissionDefinition
    {
        public required string Name { get; set; }
        public required Visibility Visibility { get; set; }
        public required List<AppliesTo> AppliesTo { get; set; } = [];
        public required List<ConditionDefinition> AllowedConditions { get; set; } = [];
    }
}