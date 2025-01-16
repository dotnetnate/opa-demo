
namespace CitizensFinancialGroup.Elements.ApplicationModel.CQRS.Commands {
    public interface ICommandResultHandler<TResult> {
        TResult? HandleCommandResult(CommandResult result);
    }
}
