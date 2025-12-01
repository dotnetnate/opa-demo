using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CitizensFinancialGroup.Elements.CQRS.Commands {
  
    public class SuccessCommandResultHandler<TResult> : CommandResultHandlerBase<TResult> {
        public SuccessCommandResultHandler() {
        }

        
        protected override bool ShouldHandleResult(CommandResult result) {
            return result.Status == CommandStatus.Succeeded;
        }

        protected override TResult? CreateResult(object model) {
            return base.CreateResult(model);
        }

        protected override TResult? CreateResult() {
            return base.CreateResult();
        }
    }
}
