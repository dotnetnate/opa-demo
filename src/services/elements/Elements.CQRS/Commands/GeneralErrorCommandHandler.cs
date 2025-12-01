using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace NOCO.Elements.CQRS.Commands {
    public class GeneralErrorCommandResultHandler<TResult> : CommandResultHandlerBase<TResult> {
        public GeneralErrorCommandResultHandler() {
        }

        override protected bool ShouldHandleResult(CommandResult result) {
            return result.Status == CommandStatus.Failed && (result.FailureCategory == CommandFailureCategory.GeneralRuntimeError || result.FailureCategory == CommandFailureCategory.None);
        }
    }
}
