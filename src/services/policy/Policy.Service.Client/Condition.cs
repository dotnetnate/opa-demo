using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policy.Service.Client {
    /// <summary>
    /// Represents a condition that can be evaluated against a policy.
    /// </summary>
    public class Condition {
        public InputSources ContextSource { get; set; }
        /// <summary>
        /// Gets or sets the attribute name to be evaluated (e.g., "amount").
        /// </summary>
        public required string ContextAttributePath { get; set; }

        /// <summary>
        /// Gets or sets the operator to be used in the evaluation (e.g., "eq", "gte").
        /// </summary>
        public Operators Operator { get; set; }

        /// <summary>
        /// Gets or sets the value to compare against.
        /// </summary>
        public required object Value { get; set; }
    }
}
