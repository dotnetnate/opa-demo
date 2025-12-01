using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policy.Service.Client {

    /// <summary>
    /// Represents the operations that can be performed during evaluation of a condition on a rule.    
    /// </summary>
    /// <summary>
    /// Represents the operations that can be performed during evaluation of a condition on a rule.    
    /// </summary>
    public enum Operators {
        [Description("gt")]
        gt,
        [Description("lt")]
        lt,
        [Description("eq")]
        eq,
        [Description("ne")]
        ne,
        [Description("gte")]
        gte,
        [Description("lte")]
        lte
    }
}
