using System.Collections.Generic;
using System.ComponentModel;

namespace NOCO.Threvw.Policy.Service.Http.Features.Policies.Models {
    
    #region Request Models

    public class EvaluatePolicyRequest {
        public required ResourceModel Resource { get; set; }
        public required SubjectModel Subject { get; set; }
        public required ActionContextModel Action { get; set; }
    }

    public class ActionContextModel {
        /// <summary>
        /// The id of the action (e.g., "READ", "WRITE", "WIRE_TRANSFER")
        /// </summary>
        public required string Id { get; set; }
        
        /// <summary>
        /// Additional context attributes for condition evaluation
        /// </summary>
        public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();
    }

    #endregion

    #region Response Models

    public class EvaluatePolicyResponse {
        /// <summary>
        /// The policy decision
        /// </summary>
        public PolicyDecision Decision { get; set; }
        
        /// <summary>
        /// Obligations that MUST be fulfilled for this decision to be enforced
        /// </summary>
        public List<ObligationModel> Obligations { get; set; } = new();
        
        /// <summary>
        /// Advisory information that MAY be acted upon
        /// </summary>
        public List<AdviceModel> Advice { get; set; } = new();
        
        /// <summary>
        /// Additional details about the evaluation (for debugging/audit)
        /// </summary>
        public string? EvaluationDetails { get; set; }
    }

    public enum PolicyDecision {
        [Description("deny")]
        Deny = 0,
        
        [Description("permit")]
        Permit = 1,
        
        [Description("indeterminate")]
        Indeterminate = 2,
        
        [Description("notApplicable")]
        NotApplicable = 3
    }

    #endregion
}
