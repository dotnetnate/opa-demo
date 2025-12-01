using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policy.Service.Client {
    /// <summary>
    /// Represents a policy that contains a resource and a list of rules associated with it.
    /// </summary>
    public class Policy {
        /// <summary>
        /// Gets or sets the resource details.
        /// </summary>
        public required Resource Resource { get; set; }
        /// <summary>
        /// Gets or sets the list of rules associated with the resource.
        /// </summary>
        public required ICollection<Rule> Rules { get; set; } = [];
    }
}
