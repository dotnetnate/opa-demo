
namespace CitizensFinancialGroup.Elements.ApplicationModel.Commands {
    public interface ICommandResultHandler<TResult> {
        TResult? HandleCommandResult(CommandResult result);
    }
}
