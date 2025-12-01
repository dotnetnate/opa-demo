using System;
using System.Collections.Generic;

namespace NOCO.Elements.CQRS.Commands {
    public class InvalidOperationCommandResultHandler<TResult> : CommandResultHandlerBase<TResult> {
        public InvalidOperationCommandResultHandler() {
        }


        protected override bool ShouldHandleResult(CommandResult result) {
            return result.Status == CommandStatus.Failed && result.FailureCategory == CommandFailureCategory.InvalidOperation;
        }        
    }
}
