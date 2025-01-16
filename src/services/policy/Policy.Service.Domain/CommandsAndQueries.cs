using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Elements.ApplicationModel.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    #region Commands

    public class CreatePolicyCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource to associate with the policy
        public List<Rule> Rules { get; set; } = new(); // The rules to include in the policy
    }

    public class UpdatePolicyCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
        public List<Rule> Rules { get; set; } = new(); // The updated rules for the policy
    }

    public class DeletePolicyCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy to delete
    }

    public class AddOrUpdateRuleCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
        public required Rule Rule { get; set; } // The rule to add or update
    }

    public class DeleteRuleCommand : CommandBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
        public required string SubjectId { get; set; } // The subject ID of the rule to delete
    }

    #endregion

    #region Queries

    public class FindPoliciesQuery : QueryBase {
        public Resource? ResourceFilter { get; set; }
        public Subject? SubjectFilter { get; set; }        
        public int PageNumber { get; set; } = 1; // The page number for pagination
        public int PageSize { get; set; } = 10; // The number of items per page
    }

    public class FindPolicyByResourceQuery : QueryBase {
        public required Resource Resource { get; set; } // The resource identifying the policy
    }

    #endregion
}
