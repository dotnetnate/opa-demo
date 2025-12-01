using System;
using System.Collections.Generic;
using System.Text;

namespace NOCO.Elements.CQRS.Commands{
    public enum CommandFailureCategory {
        None = 0,
        ResourceNotFound = 1,
        InsufficientPermissions = 2,
        ParameterValidation = 4,
        InvalidOperation = 8,
        VersionConlfict = 16,        
        RejectedContent = 32,
        GeneralRuntimeError = 64,
        ResourceConflict = 128
    }
}
