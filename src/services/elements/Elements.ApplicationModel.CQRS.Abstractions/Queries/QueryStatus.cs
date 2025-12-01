using System;
using System.Collections.Generic;
using System.Text;

namespace NOCO.Elements.ApplicationModel.CQRS.Queries {
    public enum QueryStatus {
        Succeeded = 0,        
        Failed = 200,
        Unknown = 300        
    }
}
