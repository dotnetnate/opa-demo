using CitizensFinancialGroup.Elements.ApplicationModel.Queries;
using CitizensFinancialGroup.Threvw.Policies.Domain;

namespace CitizensFinancialGroup.Threvw.Policies.Application {
    public class FindPolicyByResourceQuery : QueryBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
    }
}
