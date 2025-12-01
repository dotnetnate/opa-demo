using System;
using System.Collections.Generic;
using System.Text;

namespace NOCO.Elements.CQRS.Commands {
    public enum QueryStatus {
        Succeeded = 0,        
        Failed = 200,
        Unknown = 300        
    }
}
