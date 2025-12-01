using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {
    /// <summary>
    /// Represents a subject within the policy domain.
    /// </summary>
    public class Subject : ScopedIdentity {        
        /// <summary>
        /// Gets or sets the optional attributes for the subject.
        /// </summary>
        public Dictionary<string,string>? ExtendedProperties { get; set; } = new(); // Optional attributes for the resource
    }
}
