using System;
using System.Collections.Generic;
using System.Text;

namespace NOCO.Elements.CQRS.Commands{
    public enum QueryFailureCategory {
        None = 0,
        ResourceNotFound = 1,
        InsufficientPermissions = 2,
        ParameterValidation = 4,        
        GeneralRuntimeError = 64        
    }
}
