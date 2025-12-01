using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Threvw.Policies.Domain;

namespace NOCO.Threvw.Policies.Application {
    public class AddOrUpdateRuleCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
        public required Rule Rule { get; set; } // The rule to add or update
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }
}
