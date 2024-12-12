using System;
using System.Collections.Generic;
using System.Text;

namespace CitizensFinancialGroup.Elements.CQRS.Commands {
    public enum CommandStatus {
        Succeeded = 0,
        Received = 100,
        Failed = 200,
        Unknown = 300,        
    }
}
