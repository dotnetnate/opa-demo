


namespace NOCO.Elements.ApplicationModel.Commands {
    public class BadRequestCommandResultHandler<TResult> : CommandResultHandlerBase<TResult> {        
        protected override bool ShouldHandleResult(CommandResult result) {
            return result.Status == CommandStatus.Failed && result.FailureCategory == CommandFailureCategory.ParameterValidation;
        }

        
    }
}
