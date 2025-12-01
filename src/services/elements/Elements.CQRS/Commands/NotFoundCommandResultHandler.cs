using System.ComponentModel.DataAnnotations;

namespace NOCO.Elements.CQRS.Commands {

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
