using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Threvw.Policies.Domain;

namespace CitizensFinancialGroup.Threvw.Policies.Application {
    public class AddOrUpdateRuleCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
        public required Rule Rule { get; set; } // The rule to add or update
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }
}
