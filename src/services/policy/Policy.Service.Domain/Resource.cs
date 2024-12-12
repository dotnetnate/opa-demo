using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    public class Resource {
        public string ResourceId { get; set; } // The unique identifier for the resource
        public string ResourceType { get; set; } // The type of resource (e.g., "application")
        public List<string> Attributes { get; set; } = new(); // Optional attributes for the resource
    }
}
