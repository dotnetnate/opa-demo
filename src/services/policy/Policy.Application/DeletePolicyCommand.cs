using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Threvw.Policies.Domain;

namespace CitizensFinancialGroup.Threvw.Policies.Application {
    public class DeletePolicyCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy to delete
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }
}
