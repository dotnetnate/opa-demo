using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Threvw.Policies.Domain;

namespace NOCO.Threvw.Policies.Application {
    public class DeleteRuleCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
        public required Subject Subject { get; set; } // The subject (authority + identifier) of the rule to delete
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }
}
