using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Threvw.Policies.Domain;

namespace CitizensFinancialGroup.Threvw.Policies.Application {
    public class CreatePolicyCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource to associate with the policy
        public List<Rule> Rules { get; set; } = new(); // The rules to include in the policy
    }
}
