using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CitizensFinancialGroup.Elements.ApplicationModel.Commands {
    public class ConflictCommandResultHandler : ConflictCommandResultHandler<IActionResult> {

        public ConflictCommandResultHandler() {
        }

        protected override bool ShouldHandleResult(CommandResult result) {
            return result.Status == CommandStatus.Failed && result.FailureCategory == CommandFailureCategory.VersionConlfict;
        }
    }
}
