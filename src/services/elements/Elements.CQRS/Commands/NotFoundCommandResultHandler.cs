using System.ComponentModel.DataAnnotations;

namespace CitizensFinancialGroup.Elements.CQRS.Commands {

    public class NotFoundCommandResultHandler<TResult> : CommandResultHandlerBase<TResult> {
        public NotFoundCommandResultHandler() {
        }

        protected override bool ShouldHandleResult(CommandResult result) {
            return result.Status == CommandStatus.Failed && result.FailureCategory == CommandFailureCategory.ResourceNotFound;
        }

        override protected TResult? CreateResult() {
            return default;
        }


    }
}
