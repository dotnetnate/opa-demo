using System;
using System.Collections.Generic;
using System.Text;

namespace CitizensFinancialGroup.Elements.CQRS.Commands {
    public enum QueryStatus {
        Succeeded = 0,        
        Failed = 200,
        Unknown = 300        
    }
}
