using NOCO.Elements.ApplicationModel.Queries;
using NOCO.Threvw.Policies.Domain;

namespace NOCO.Threvw.Policies.Application {
    public class FindPolicyByResourceQuery : QueryBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
    }
}
