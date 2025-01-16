namespace CitizensFinancialGroup.Elements.ApplicationModel.Commands {
    public class UnauthorizedCommandResultHandler<TResult> : CommandResultHandlerBase<TResult> {

        protected override bool ShouldHandleResult(CommandResult result) {
            return result.Status == CommandStatus.Failed && result.FailureCategory == CommandFailureCategory.InsufficientPermissions;
        }

        protected override TResult? CreateResult() {
            return base.CreateResult(); // implement
        }
    }
}
