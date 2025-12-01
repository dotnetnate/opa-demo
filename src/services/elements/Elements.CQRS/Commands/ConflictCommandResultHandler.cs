using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NOCO.Elements.CQRS.Commands {
    public class ConflictCommandResultHandler<TResult> : CommandResultHandlerBase<TResult> {
        public ConflictCommandResultHandler() {
        }

        protected override bool ShouldHandleResult(CommandResult result) {
            return result.Status == CommandStatus.Failed && result.FailureCategory == CommandFailureCategory.VersionConlfict;
        }

    }
}
