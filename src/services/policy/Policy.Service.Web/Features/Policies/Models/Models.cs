using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;

namespace CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Models {
    #region Requests

    public class FindPoliciesRequest {
        public string ResourceId { get; set; } // Optional filter by resource ID
        public string ResourceType { get; set; } // Optional filter by resource type
        public string SubjectId { get; set; } // Optional filter by subject ID
        public int PageNumber { get; set; } = 1; // Pagination page number
        public int PageSize { get; set; } = 10; // Pagination page size
    }

    public class FindPolicyByResourceRequest {
        public string ResourceId { get; set; } // Resource ID for the policy
        public string ResourceType { get; set; } // Resource type for the policy
    }

    public class CreatePolicyRequest {
        public ResourceModel Resource { get; set; } // Resource details for the policy
        public List<RuleModel> Rules { get; set; } = new(); // List of rules for the policy
    }

    public class UpdatePolicyRequest {
        public List<RuleModel> Rules { get; set; } = new(); // Updated list of rules for the policy
    }

    public class AddOrUpdateRuleRequest {
        public RuleModel Rule { get; set; } // Rule details to add or update
    }

    #endregion

    #region Models

    public class DeleteRuleRequest {
        public string ResourceId { get; set; } // The resource ID identifying the policy
        public string ResourceType { get; set; } // The resource type identifying the policy
        public string SubjectId { get; set; } // The subject ID of the rule to delete
    }

    public class PolicyModel {
        public ResourceModel Resource { get; set; } // Resource associated with the policy
        public List<RuleModel> Rules { get; set; } = new(); // Rules within the policy
    }

    public class ResourceModel {
        public string ResourceId { get; set; } // Unique identifier for the resource
        public string ResourceType { get; set; } // Type of the resource
    }

    public class RuleModel {
        public SubjectModel Subject { get; set; } // Subject details for the rule
        public List<PrivilegeModel> Privileges { get; set; } = new(); // Privileges granted by the rule
    }

    public class SubjectModel {
        public string Identifier { get; set; } // Unique identifier for the subject (e.g., user ID)
        public string Type { get; set; } // Type of subject (e.g., "user", "role")
        public string Authority { get; set; } // Authority governing the subject (e.g., "https://auth0.com/")
    }

    public class PrivilegeModel {
        public string Name { get; set; } // Name of the privilege (e.g., "PERMISSION_1")
        public List<ConditionModel> Conditions { get; set; } = new(); // Conditions for the privilege
    }

    public class ConditionModel {
        public string Attribute { get; set; } // Attribute name (e.g., "amount")
        public string Operator { get; set; } // Operator (e.g., "eq", "gt")
        public object Value { get; set; } // Value to compare against
    }

    #endregion

    #region Error Handling

    public class ErrorResult {
        public InternalValidationResult ValidationResult { get; set; } // Validation result details
        public IEnumerable<string> Errors { get; set; } = new List<string>(); // List of error messages
    }

    public class InternalValidationResult {
        public bool IsValid() => Errors.Count == 0; // Check if validation is successful
        public List<InternalValidationError> Errors { get; set; } = new(); // Validation errors
    }

    public class InternalValidationError {
        public string PropertyName { get; set; } // Name of the property with the error
        public string ErrorMessage { get; set; } // Error message
    }

    #endregion

}
