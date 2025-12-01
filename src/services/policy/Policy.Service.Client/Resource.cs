using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policy.Service.Client {
    /// <summary>
    /// Represents a resource with an identifier, scope, type, and optional extended properties.
    /// </summary>
    public class Resource : ScopedIdentity {
        /// <summary>
        /// Gets or sets the optional attributes for the resource.
        /// </summary>
        public Dictionary<string, string>? ExtendedProperties { get; set; } = new(); // Optional attributes for the resource
    }
}
