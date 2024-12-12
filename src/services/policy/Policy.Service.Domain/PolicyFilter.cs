using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Represents filtering criteria for finding policies in the repository.
    /// </summary>
    public class PolicyFilter {
        /// <summary>
        /// Optional: The ID of the resource to filter policies by.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// Optional: The type of the resource to filter policies by.
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// Optional: The identifier of the subject to filter policies by.
        /// </summary>
        public string SubjectId { get; set; }
    }
}
