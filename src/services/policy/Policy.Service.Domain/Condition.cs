using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {
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
    /*
    public abstract class Operand<T> {        
        public required T Value { get; set; }
    }
    public class ConstantOperand<T> : Operand<T> { }    

    public class ContextReferenceOperand : Operand<string> {
        public InputSources Source { get; set; }        
    }

    public class Expression<T> {
        public required Operand<T> Left { get; set; }
        public Operator Operator { get; set; }
        public required Operand<T> Right { get; set; }
    } */
}
