using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Threvw.Policies.Domain;

namespace CitizensFinancialGroup.Threvw.Policies.Application {
    public class UpdatePolicyCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
        public List<Rule> Rules { get; set; } = new(); // The updated rules for the policy
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }
}
