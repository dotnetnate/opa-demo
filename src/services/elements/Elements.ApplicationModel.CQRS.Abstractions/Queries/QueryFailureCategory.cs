using System;
using System.Collections.Generic;
using System.Text;

namespace CitizensFinancialGroup.Elements.ApplicationModel.CQRS.Queries {
    public enum QueryFailureCategory {
        None = 0,
        ResourceNotFound = 1,
        InsufficientPermissions = 2,
        ParameterValidation = 4,        
        GeneralRuntimeError = 64        
    }
}
