using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Threvw.Policies.Domain;

namespace NOCO.Threvw.Policies.Application {
    public class CreatePolicyCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource to associate with the policy
        public List<Rule> Rules { get; set; } = new(); // The rules to include in the policy
    }
}
